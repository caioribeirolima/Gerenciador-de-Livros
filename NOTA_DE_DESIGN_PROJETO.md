# Nota de design: cadastro de usuários e limite de empréstimos (Projeto da Matéria 10)

## O que foi feito

- `POST /api/usuarios` (nome, e-mail, CPF) e `GET /api/usuarios/{id}`.
- `Cpf` (`Application/Common/Cpf.cs`): `Normalizar` (só dígitos), `EhValido` (formato, sequência repetida, dígitos verificadores) e `Mascarar`.
- Regra de validator `CpfObrigatorio` em `Common/Validation`, usada no `CriarUsuarioCommandValidator`.
- `CriarUsuarioHandler`: normaliza, devolve `Conflito` se o CPF já existe e trata a corrida com o índice único.
- `RegistrarEmprestimoHandler`: `Conflito` quando o usuário já tem `MaximoEmprestimosAtivos` empréstimos sem devolução (padrão 3, em `appsettings.json`).

## Decisões de banco

- **`Cpf` é anulável no banco** (`nvarchar(11)`), embora o cadastro novo o exija. Motivo: já existem usuários sem CPF. Uma coluna obrigatória exigiria um valor padrão, e vários usuários com o mesmo valor (por exemplo `""`) violariam o índice único. Inventar CPFs falsos para eles seria pior.
- **Índice único com filtro** `[Cpf] IS NOT NULL` (o SQL Server gera o filtro sozinho para coluna anulável). Os usuários antigos não colidem entre si; qualquer cadastro novo colide se repetir o CPF.
- **Só dígitos no banco.** `123.456.789-09` e `12345678909` são o mesmo CPF; se o handler gravasse como veio, o índice trataria como valores diferentes.
- A migration e o snapshot foram **escritos à mão**, porque o ambiente onde trabalhei não tem `dotnet ef`. Conferir com `dotnet ef migrations has-pending-model-changes`. Se acusar diferença, apagar `20261020233000_AdicionaCpfUsuario*` e gerar de novo com `migrations add AdicionaCpfUsuario`.

## Quem decide o quê

| Regra | Onde | Resposta |
|---|---|---|
| Nome vazio ou longo, e-mail mal formado, CPF com formato ou dígitos errados | Validator | 400 |
| CPF já cadastrado | Handler (`ExisteComCpfAsync`) + índice único | 409 |
| Usuário no limite de empréstimos | Handler | 409 |
| Usuário inexistente | Handler | 404 |

O CPF repetido não é validação: depende do estado do banco, e outra requisição pode mudar esse estado a qualquer momento.

## (a) A corrida do limite de empréstimos

**O problema.** O handler conta os empréstimos ativos e só depois grava. Duas requisições simultâneas do mesmo usuário (que tem 2 ativos) leem 2, as duas passam, as duas gravam, e o usuário termina com 4. Diferente do CPF, **o banco não tem como barrar isso sozinho**: não existe índice único para "no máximo 3 linhas por usuário".

**Opções:**

1. **Transação `Serializable` (ou `UPDLOCK, HOLDLOCK` na contagem).** Resolve, mas bloqueia mais do que o necessário e abre risco de deadlock.
2. **Contador no `Usuario` com `UPDATE` condicional atômico** (`SET Ativos = Ativos + 1 WHERE Id = @id AND Ativos < @max`, via `ExecuteUpdateAsync`). É a mais robusta: o banco decide e ninguém passa. O custo é manter um número derivado que precisa ser decrementado na devolução (e consertado se algo falhar no meio): duas fontes de verdade.
3. **`rowversion` (token de concorrência) em `Usuario`.** O handler "toca" o usuário ao registrar o empréstimo. Dois registros simultâneos leem a mesma versão e o segundo recebe `DbUpdateConcurrencyException`, que o handler converte em `Conflito`. Segue o mesmo padrão do CPF (a exceção só vira 409 se a causa for essa).

**Escolha.** A 3, se for preciso fechar a corrida: é a menor mudança e reaproveita o padrão `catch` + reconferência. **Não implementei** porque o requisito pedia a análise, e porque, numa biblioteca, uma violação rara do limite (usuário com 4 em vez de 3) tem consequência baixa. Para limites que envolvem dinheiro ou estoque, eu implementaria a opção 2.

## (b) CPF é dado pessoal (LGPD): onde ele poderia vazar

| Ponto | Risco | Decisão |
|---|---|---|
| **Mensagens de erro** | "O CPF 529... já está cadastrado" colocaria o dado na resposta e em qualquer log do cliente/proxy. | As mensagens (validator e handler) nunca repetem o valor. Há teste para os dois (`Cpf_Invalido_MensagemNaoRepeteOValorInformado`, `Criar_CpfJaCadastrado_MensagemNaoVazaOCpf`). |
| **`LoggingBehavior`** | Se logar o objeto da requisição (`{@Request}`), o CPF vai para o log. | Hoje ele loga **só o nome do tipo** (`typeof(TRequest).Name`). Regra: não passar a requisição inteira para o logger. |
| **Resposta do GET** | Devolver o CPF inteiro para qualquer cliente que saiba o id. | Devolve `CpfMascarado` (`***.982.247-**`). A API nunca devolve o CPF completo. |
| **URL e query string** | URLs vão para logs de acesso do servidor, proxies e histórico do navegador. | O CPF só trafega no **corpo** do POST. Por isso não existe `GET /api/usuarios?cpf=...`; uma busca por CPF teria que ser `POST` (ou receber o dado por outro meio). |
| **Erros do MVC** (JSON malformado, tipo errado) | Podem ecoar parte da entrada. | O texto do `[ApiController]` não ecoa o valor (`The JSON value could not be converted...`); conferir se isso muda ao trocar de versão. |

**Fora do escopo, mas vale registrar:** o CPF fica em texto puro na coluna. Para dado pessoal em produção, avaliar criptografia da coluna (Always Encrypted) ou, no mínimo, restringir quem lê a tabela.

## Limites desta entrega

- Escrito sem poder compilar nem rodar `dotnet test` (o ambiente bloqueia o NuGet). A revisão foi só por leitura do código; rodar a suíte inteira antes do merge.
- Não há teste de integração com SQL Server real: o `InMemory` **não impõe o índice único**. O caminho da corrida é coberto por um teste que simula a falha no `Commit`, não pela violação real do índice.
