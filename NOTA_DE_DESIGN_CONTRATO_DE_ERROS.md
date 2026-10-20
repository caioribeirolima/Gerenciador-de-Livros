# Nota de design: o contrato dos erros de validação

## Resposta real

`POST /api/Livros` com título vazio e ano futuro (2027, com o relógio em 2026), feito contra a API rodando localmente:

```json
{"title":"Erro de validação","status":400,"errors":{"Titulo":["O título do livro é obrigatório."],"AnoPublicacao":["Ano de publicação não pode ser maior que o ano atual."]}}
```

Corpo enviado:

```json
{"titulo":"","anoPublicacao":2027,"categoriaId":1,"editoraId":1,"autorIds":[]}
```

## 1. `Titulo` ou `titulo`?

As chaves saem como **`Titulo`**, em PascalCase, embora o corpo da requisição use `titulo`.

**De onde vem:**

1. O FluentValidation preenche `PropertyName` com o nome do membro C# (`Titulo`, `AutorIds[1]`).
2. O `ValidationBehavior` agrupa as falhas por `f.PropertyName` e monta o dicionário sem alterar as chaves.
3. O `ErroDe` cria `ValidationProblemDetails` com esse dicionário.
4. O System.Text.Json do ASP.NET usa `PropertyNamingPolicy = CamelCase`, que vale para **propriedades** (`title`, `status`, `errors`), mas `DictionaryKeyPolicy` fica nulo. As **chaves de dicionário** saem como estão.

**O cliente é afetado?** Sim. Um front que liga erros a campos de formulário pelo nome que ele mesmo enviou (`errors[campo.name]`, com `name="titulo"`) não encontra `errors.titulo`, e o erro não aparece na tela. O front é obrigado a converter nomes ou a saber que este endpoint responde em PascalCase.

Pior, a API tem **três formatos** de chave para o mesmo tipo de problema. Com `"anoPublicacao":"abc"`, o erro acontece no model binding, antes do MediatR, e o `[ApiController]` responde:

```json
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"request":["The request field is required."],"$.anoPublicacao":["The JSON value could not be converted to System.Int32. Path: $.anoPublicacao | LineNumber: 0 | BytePositionInLine: 35."]},"traceId":"00-6b3e85b28b4736bb6f9d342d80b9b5d2-10cebd12d7514b60-00"}
```

Aqui aparecem `$.anoPublicacao` (caminho JSON, em camelCase), `request` (nome do parâmetro da action) e, nos erros do FluentValidation, `Titulo`. Para unificar, o caminho mais curto é converter a chave no `ValidationBehavior` com `JsonNamingPolicy.CamelCase.ConvertName` (`AutorIds[1]` → `autorIds[1]`) ou configurar `ValidatorOptions.Global.PropertyNameResolver`. Não alterei nada: mudar as chaves quebra quem já depende de `Titulo`, e essa decisão é de contrato.

## 2. `Titulo = ""` gera mensagens duplicadas?

**Não.** Com `""`, o `NotEmpty` falha, mas o `MaximumLength(200)` passa, porque o comprimento é 0. A resposta acima traz uma única mensagem para `Titulo`.

A duplicação acontece quando duas regras da mesma cadeia falham juntas. Isso já ocorre no projeto, no `ReservarLivroCommandValidator`, com `LivroId = 0`:

```
LivroId | NotEmptyValidator    | O Id do livro é obrigatório.
LivroId | GreaterThanValidator | O Id do livro deve ser maior que zero.
```

Para um `int`, `NotEmpty` significa "diferente de `default`", ou seja, diferente de 0. Por isso as duas regras falham no mesmo valor e o campo recebe duas mensagens.

**`CascadeMode`:** define se a cadeia de regras de um `RuleFor` continua depois da primeira falha.

- `Continue` (padrão): todas as regras da cadeia rodam, e cada falha vira uma mensagem. É o que produz a duplicação acima.
- `Stop`: a cadeia para na primeira falha **daquele `RuleFor`**. Os outros `RuleFor` continuam rodando, então a resposta ainda traz todos os campos com problema.

**Quando usar `Cascade(CascadeMode.Stop)`:**

- Quando uma regra só faz sentido se a anterior passou. Por exemplo, `NotNull` antes de `MaximumLength` ou de uma regra que lê `x.Algo.Length`, para não validar `null` duas vezes ou estourar exceção.
- Quando as regras se sobrepõem e a segunda só repete a primeira (`NotEmpty` + `GreaterThan(0)` em `int`). Aqui o melhor é remover o `NotEmpty`, porque `GreaterThan(0)` já cobre o 0.
- Antes de uma regra cara (`MustAsync`, regex pesada), para não executá-la sobre um valor que já foi recusado.

Também existe a configuração por classe (`ClassLevelCascadeMode`, `RuleLevelCascadeMode`) e global (`ValidatorOptions.Global.DefaultRuleLevelCascadeMode`), mas o `.Cascade(CascadeMode.Stop)` no `RuleFor` deixa a intenção visível onde a regra está escrita.

## 3. `ErrorCode` vs `ErrorMessage`

Saída do FluentValidation para os mesmos casos:

```
Titulo        | NotEmptyValidator         | O título do livro é obrigatório.
AnoPublicacao | LessThanOrEqualValidator  | Ano de publicação não pode ser maior que o ano atual.
```

- **`ErrorMessage`** é texto para pessoas: em português, com artigo e pontuação. Ele muda sempre que alguém corrige a redação, traduz a API ou extrai um helper como o `NomeObrigatorio`.
- **`ErrorCode`** é um identificador para máquinas. O padrão é o nome do validador (`NotEmptyValidator`, `MaximumLengthValidator`), e `.WithErrorCode("TITULO_OBRIGATORIO")` define um código de domínio estável.

**Por que o cliente deveria decidir pelo código:** um `if (msg === "O título do livro é obrigatório.")` quebra na primeira revisão de texto, e o compilador não avisa. O código é contrato: o front pode escolher a própria mensagem (traduzida, mais curta, com ícone), destacar o campo certo ou disparar um comportamento específico (por exemplo, "nome já cadastrado" oferece abrir o registro existente). A mensagem do servidor fica como texto padrão para o caso de o front não conhecer o código.

**Limitação atual:** a API **não expõe o `ErrorCode`**. O `ValidationBehavior` guarda só `f.ErrorMessage` (`g.Select(f => f.ErrorMessage)`), e `ValidationProblemDetails.Errors` é `IDictionary<string, string[]>`, ou seja, só textos. Para o cliente poder usar códigos, o behavior precisa levar o `ErrorCode` adiante e o `ErroDe` precisa publicá-lo, por exemplo num campo extra `codigos` em `problem.Extensions`. Isso muda o `Result` e o contrato da API e não está implementado.
