# Nota de design: Renovação de empréstimo

Regras (numeração do enunciado):

1. Só renova empréstimo em aberto (não devolvido).
2. No máximo 2 renovações por empréstimo.
3. Não renova empréstimo atrasado (compara só a data).
4. Não renova se existe reserva `Ativa` ou `Notificada` de outro usuário para o livro.
5. A renovação soma `DiasParaDevolucao` (configuração) à data prevista atual, não a "hoje".

## 1. Onde mora cada regra

| O quê | Onde | Por quê |
|---|---|---|
| Regras 1, 2 e 3 | `Emprestimo.MotivoParaNaoRenovar(hoje)` | Dependem só de campos do próprio empréstimo (`DataDevolucaoReal`, `Renovacoes`, `DataDevolucaoPrevista`). Ficando na entidade, qualquer caminho que renove passa por elas, e o limite `LimiteRenovacoes = 2` mora num lugar só. |
| Regra 5 | `Emprestimo.Renovar(dias, hoje)` | A mudança de estado (`DataDevolucaoPrevista += dias`, `Renovacoes++`) é da entidade. `Renovar` chama `MotivoParaNaoRenovar` de novo e lança `InvalidOperationException` se ela recusar, então não há como renovar pulando as regras 1 a 3. |
| Regra 4 | `IReservaRepository.ExisteReservaPendenteDeOutroUsuarioAsync` | Precisa do banco. `Emprestimo` não tem a coleção de reservas, e carregá-la só para contar seria caro. O repositório faz um `AnyAsync` traduzido para SQL, coberto pelo índice `(LivroId, Status, DataReserva)` que já existe. |
| Livro excluído (seção 3) | Handler, via `ILivroRepository.BuscarPorIdAsync` | Também precisa do banco. |
| Orquestração | `RenovarEmprestimoHandler` | Busca o empréstimo, chama a entidade, consulta os repositórios, lê `DiasParaDevolucao` de `IOptions<EmprestimoSettings>` e faz o commit pelo `IUnitOfWork`. |

Duas decisões menores:

- A entidade recebe `hoje` por parâmetro em vez de ler `DateTime.Now`. Isso deixa a regra 3 testável e põe o relógio num único ponto (o handler). A comparação é `DataDevolucaoPrevista.Date < hoje.Date`: um empréstimo que vence hoje ainda pode ser renovado a qualquer hora do dia (teste 5).
- A entidade não recebe `EmprestimoSettings`. A configuração é assunto da aplicação, e a entidade recebe só o número de dias.

Ordem no handler: primeiro as regras da entidade, que não custam consulta; depois livro excluído e reserva, que vão ao banco.

## 2. Concorrência

**Dois PUT simultâneos (duplo clique).** As duas requisições leem o mesmo estado, por exemplo `Renovacoes = 0` e `DataDevolucaoPrevista = D`. As duas passam pelas regras e gravam `Renovacoes = 1` e `D + 7`. Como os valores são iguais, a última escrita não estraga nada: o resultado final é uma renovação, e não duas. O defeito é que as duas respondem 204, e o cliente acha que renovou duas vezes. O limite de 2 nunca é ultrapassado, porque as duas partem da mesma base.

Se o segundo clique chega depois do commit do primeiro, ele lê o estado já renovado e faz uma segunda renovação legítima (`D + 14`, `Renovacoes = 2`). Isso não é corrida, é uma requisição repetida, e nenhum controle de concorrência impede.

**Reserva criada no mesmo instante da renovação.** A renovação consulta as reservas e não acha nenhuma. Antes do commit dela, `ReservarLivroHandler` insere uma reserva `Ativa`. A renovação então grava `D + 7`, e quem acabou de reservar espera uma semana a mais, o que contraria a regra 4. Um `RowVersion` em `Emprestimo` não resolve esse caso, porque a reserva não altera a linha do empréstimo.

**Como proteger:**

- Dois PUT simultâneos: um `[Timestamp] byte[] RowVersion` em `Emprestimo`. O segundo `SaveChanges` falharia com `DbUpdateConcurrencyException`, que o handler converteria em `Result.Falha("tente novamente")`. Para o clique repetido depois do commit, seria preciso uma chave de idempotência enviada pelo cliente.
- Reserva concorrente: serializar renovação e reserva por livro. Uma opção é `sp_getapplock` com o `LivroId` como recurso, nas duas operações. Outra é rodar a renovação numa transação `Serializable`: a consulta de reservas trava o intervalo do índice `(LivroId, Status, DataReserva)`, e a inserção da reserva espera o commit da renovação. A terceira é as duas operações tocarem um `RowVersion` em `Livro`.

**Implementado: nada disso.** Nenhuma das proteções acima foi implementada. As duas janelas existem no código atual. A do duplo clique é benigna para os dados; a da reserva concorrente pode atrasar quem reservou em até `DiasParaDevolucao` dias.

## 3. Livro excluído com empréstimo aberto

**Decisão: não pode renovar.** O handler responde `Falha` ("foi excluído do acervo... apenas devolvido"), sem alterar o empréstimo (teste 8).

Justificativa:

- `Excluido` é o soft delete do livro: a biblioteca tirou o exemplar do acervo. Renovar é conceder mais um período de posse, e não faz sentido conceder isso para um item que a biblioteca quer de volta.
- Fica coerente com o resto do sistema. `RegistrarEmprestimoHandler` e `ReservarLivroHandler` usam `BuscarPorIdAsync`, que respeita o filtro global, então um livro excluído não pode ser emprestado nem reservado. A renovação usa o mesmo método e segue a mesma regra.
- A devolução continua possível: `DevolverEmprestimoHandler` usa `BuscarPorIdIncluindoExcluidosAsync`, e o teste `Devolver_LivroExcluidoComEmprestimoAberto_DevolucaoFunciona` cobre isso. O usuário não fica preso com o livro, só não pode estendê-lo.
