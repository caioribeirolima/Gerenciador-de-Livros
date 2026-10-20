# Nota de design: AutorIds na criação de livro

Regras em `CriarLivroCommandValidator`:

1. Lista vazia é permitida (livro sem autores).
2. Cada Id precisa ser maior que zero (`RuleForEach`).
3. Ids repetidos são proibidos (`RuleFor(...).Must(Distinct)`).

Testes em `LivrosTests`: `CriarLivro_SemAutores_Sucesso`, `CriarLivro_AutoresDuplicados_RetornaValidacaoSemGravar` e `CriarLivro_AutorIdZero_RetornaValidacaoNoIndiceSemGravar`.

## Chave do erro na resposta

O `RuleForEach` gera a chave com o índice do item (`AutorIds[1]`), e o `Must` sobre a lista gera a chave da propriedade (`AutorIds`). O ASP.NET não converte as chaves do dicionário `errors` para camelCase, então elas saem em PascalCase, como no command.

`POST /api/Livros` com `"autorIds": [1, 0]`:

```json
{"title":"Erro de validação","status":400,"errors":{"AutorIds[1]":["O Id de um dos autores é inválido."]}}
```

`POST /api/Livros` com `"autorIds": [1, 1]`:

```json
{"title":"Erro de validação","status":400,"errors":{"AutorIds":["Não é permitido autores duplicados."]}}
```
