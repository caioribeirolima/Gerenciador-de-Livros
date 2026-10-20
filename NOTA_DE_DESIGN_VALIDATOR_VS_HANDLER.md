# Nota de design: validator ou handler?

Critério: o validator olha **só a forma da entrada** (dá para decidir sem ir ao banco). Tudo que depende do **estado do sistema** fica no handler.

## Cenários

| Cenário | Onde | HTTP | Neste projeto |
|---|---|---|---|
| Título com 201 caracteres | Validator | 400 | `CriarLivroCommandValidator`: `MaximumLength(200)` |
| Reservar livro que já está emprestado ao próprio usuário | Handler | 409 | `ReservarLivroHandler`: `ExisteEmprestimoAbertoDoUsuarioAsync` → `Result.Conflito` |
| `CategoriaId` que não existe | Handler | 404 | **Hoje devolve 500**: `CriarLivroHandler` não confere a categoria e o insert quebra na FK `FK_Livros_Categorias_CategoriaId`. O handler deveria buscar a categoria e devolver `NaoEncontrado`. |
| `UsuarioId = -5` | Validator | 400 | `ReservarLivroCommandValidator`: `GreaterThan(0)`. Um Id negativo é entrada malformada, não um usuário inexistente. |
| Autor com nome já cadastrado | Handler | 409 | `CriarAutorHandler` → `Result.Conflito` (teste `CriarAutor_NomeDuplicadoCaseDiferente_Conflito`) |
| Ano de publicação 2031 | Validator | 400 | `CriarLivroCommandValidator`: `LessThanOrEqualTo(_ => DateTime.Now.Year)` |

## Por que `MustAsync(existe no banco)` no validator costuma ser má ideia

**1. Responde o código HTTP errado.** Toda falha de validator vira `TipoErro.Validacao`, e o `ErroDe` transforma isso em 400. "Categoria 7 não existe" é 404, e "nome já cadastrado" é 409. Colocar essas checagens no validator desfaz os erros tipados (`NaoEncontrado`, `Conflito`) que o projeto acabou de introduzir: o cliente deixa de distinguir "você mandou lixo" de "o recurso sumiu".

**2. Não garante nada e consulta duas vezes.** O validator roda antes do handler, em outro momento. Entre a checagem e o `CommitAsync`, outra requisição pode criar ou apagar o registro (check-then-act). Por isso o handler precisa tratar o caso de qualquer jeito, como o `ReservarLivroHandler` faz com o `catch (DbUpdateException)` e a constraint única. O `MustAsync` só acrescenta uma ida ao banco a mais, e muitas vezes o handler busca a mesma entidade logo em seguida (`BuscarPorIdAsync`).

Um terceiro risco, específico deste projeto: o `ValidationBehavior` roda os validators com `Task.WhenAll`. Se duas validações do mesmo request fizerem `MustAsync` no mesmo `DbContext` com escopo, o EF lança "A second operation was started on this context".

## Quando seria aceitável

Quando o "banco" é um **catálogo de referência que não muda durante a operação** e a resposta certa realmente é 400. Por exemplo: conferir se um código de idioma ou uma sigla de UF existe numa tabela fixa, de preferência em cache. Nesse caso a checagem é sobre a forma da entrada ("isso não é um valor permitido"), não sobre o estado do sistema. Não há corrida que importe, e o handler não precisaria daquele registro para nada.
