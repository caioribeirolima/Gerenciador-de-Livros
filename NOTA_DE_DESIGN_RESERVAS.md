# Nota de design: Reservas

## 1. Onde a Posicao é calculada, e por quê

A `Posicao` não é gravada no banco. Ela é calculada na leitura, em `Application/Common/Mappings/ReservaMappings.cs`, dentro da própria query SQL (uma subquery `COUNT` correlacionada, traduzida pelo EF):

- reserva `Notificada` → posição 1 (é a vez dela);
- reserva `Ativa` → 1 + quantas reservas do mesmo livro estão `Notificada` ou são `Ativa` mais antigas (`DataReserva` menor; empate desempatado pelo `Id`);
- `Atendida` ou `Cancelada` → `null`.

Motivo: a fonte da verdade da fila já é `Status + DataReserva + Id`. Uma coluna `Posicao` seria uma cópia dessa informação que precisaria ser renumerada em todas as reservas seguintes a cada cancelamento, atendimento ou promoção. Isso custa N updates por operação e cria corridas (duas reservas simultâneas poderiam ler "a última posição é 3" e gravar 4 as duas). Calculando na leitura, a posição nunca fica desatualizada nem duplicada. O custo é um `COUNT` por linha, coberto pelo índice `(LivroId, Status, DataReserva)` criado na migration. As três queries usam a mesma expressão, então a regra mora em um lugar só.

## 2. O que acontece com a reserva se o Handler de aviso falhar

O fluxo da devolução é:

1. `DevolverEmprestimoHandler` faz `SaveChanges` → **commit 1** (empréstimo devolvido, livro `Disponivel = true`).
2. Publica `LivroDevolvidoNotification`.
3. `AtenderProximaReservaHandler` (que grava no banco dentro de uma Notification) muda a primeira `Ativa` para `Notificada` e faz `SaveChanges` → **commit 2**.
4. Publica `ReservaNotificadaNotification` → `AvisarUsuarioReservaDisponivelHandler`.

O publisher padrão do MediatR roda os handlers em sequência e propaga a exceção. Se o aviso falhar no passo 4:

- a reserva **continua `Notificada`**, porque o commit 2 já aconteceu; nada é desfeito;
- o usuário **nunca é avisado**, e o livro fica guardado para alguém que não sabe disso. Como não há prazo de retirada, ele fica preso indefinidamente;
- a exceção sobe até o controller, e o cliente recebe **500** embora a devolução tenha dado certo. Se ele tentar de novo, recebe "já foi devolvido" (guarda adicionada no `DevolverEmprestimoHandler`).

O teste `Devolver_QuandoAvisoFalha_DevolucaoEReservaNotificadaJaEstaoGravadas` comprova esse comportamento.

Casos vizinhos com o mesmo problema de raiz (dois commits separados):

- se o `AtenderProximaReservaHandler` falhar entre o commit 1 e o 2, o livro volta à estante sem ninguém `Notificada`, e quem chegar primeiro empresta, furando a fila;
- se outro handler de `LivroDevolvidoNotification` (por exemplo o `VerificarAtrasoDevolucaoHandler`) lançar antes, a promoção nem roda.

Como resolver (não implementado aqui):

- **Outbox:** gravar um "evento pendente" na mesma transação da devolução e processá-lo com um worker que tenta de novo até dar certo;
- **Prazo para a `Notificada`:** por exemplo 48h, para destravar a fila se o aviso não chegar;
- **Aviso tolerante a falha:** o handler de aviso captura e registra o próprio erro, sem derrubar a requisição.

## 3. O que acontece quando duas pessoas reservam ao mesmo tempo

**Duas pessoas diferentes, mesmo livro.** As duas passam pelas validações e as duas reservas são gravadas como `Ativa`. Não há conflito: a ordem sai de `DataReserva`, com o `Id` como desempate. Como a posição é calculada na leitura, as duas nunca recebem o mesmo número.

**A mesma pessoa duas vezes (duplo clique).** A checagem "já existe reserva pendente?" é do tipo verificar-e-depois-agir: as duas requisições podem ler "não existe" antes de qualquer uma gravar. Por isso a migration cria um índice único filtrado `(LivroId, UsuarioId) WHERE Status IN ('Ativa','Notificada')`. O SQL Server barra a segunda inserção, o handler captura a `DbUpdateException` e devolve `Result.Falha`. Um segundo índice único filtrado (`LivroId WHERE Status = 'Notificada'`) garante no máximo uma `Notificada` por livro, caso uma devolução e um cancelamento tentem promover a fila ao mesmo tempo. Observação: o provider InMemory não aplica índices únicos, então esse caminho só é exercido no SQL Server.

**Reserva chegando junto com a devolução.** A reserva lê `Disponivel = false`. Enquanto isso, a devolução grava e promove uma fila ainda vazia, e só então a reserva é inserida como `Ativa`. O livro fica na estante sem ninguém `Notificada`. Nesse caso, a reserva espera a próxima devolução; se o próprio dono da reserva emprestar o livro, ela vira `Atendida`. Para fechar essa janela seria preciso serializar as operações por livro, com `sp_getapplock` ou transação `Serializable`, ou criar uma rotina que reconcilie livros disponíveis que tenham fila.

**Fora do escopo, mas relacionado.** Dois empréstimos simultâneos do mesmo livro já eram possíveis antes desta entrega, porque os dois leem `Disponivel = true`. Um `rowversion` em `Livro` resolveria.
