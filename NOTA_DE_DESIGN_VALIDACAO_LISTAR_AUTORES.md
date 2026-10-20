# Nota de design: validação de ListarAutoresQuery

## 1. Sintoma

O teste `ListarAutores_PaginaZero_RetornaValidacao` manda `ListarAutoresQuery { Pagina = 0 }` pelo `IMediator` e espera `TipoErro.Validacao`. Com a query devolvendo `List<AutorResponse>`, ele falhou assim:

```
Assert.IsType() Failure: Value is not the exact type
Expected: typeof(GerenciadorLivro.Application.Common.Result<List<AutorResponse>>)
Actual:   typeof(System.Collections.Generic.List<AutorResponse>)
```

Não houve exceção nem erro de validação. O handler rodou com `Pagina = 0`, limitou para 1 com `Math.Max` e devolveu a lista. O `ListarAutoresQueryValidator` nunca foi executado.

## 2. Por que acontece

A linha responsável é `GerenciadorLivro.Application/Common/Behaviors/ValidationBehavior.cs:8`:

```csharp
where TResponse : IFalhaDeValidacao<TResponse>
```

O `AddOpenBehavior(typeof(ValidationBehavior<,>))` registra o behavior como genérico aberto. Para cada request, o MediatR pede ao container `IEnumerable<IPipelineBehavior<TRequest, TResponse>>` já fechado, aqui `IPipelineBehavior<ListarAutoresQuery, List<AutorResponse>>`. `List<AutorResponse>` não implementa `IFalhaDeValidacao<>`, então `ValidationBehavior<ListarAutoresQuery, List<AutorResponse>>` não pode ser construído. Quando resolve um `IEnumerable<>`, o container do Microsoft.Extensions.DependencyInjection **pula em silêncio** as implementações genéricas abertas cuja constraint não é satisfeita, sem lançar exceção. O pipeline roda só com `LoggingBehavior` e `PerformanceBehavior`, que não têm a constraint.

O validator continua registrado pelo `AddValidatorsFromAssembly`, mas ninguém o pede. Ele vira código morto, e nada avisa: nem o build, nem a inicialização, nem a requisição.

A constraint existe porque o behavior precisa chamar `TResponse.Validacao(erros)` (membro `static abstract` de `IFalhaDeValidacao<TSelf>`). Ele só sabe fabricar uma resposta de falha se a resposta for um `Result`.

## 3. Saída escolhida: (a) a query devolve `Result<List<AutorResponse>>`

Mudanças:

- `ListarAutoresQuery : IRequest<Result<List<AutorResponse>>>`
- `ListarAutoresHandler` devolve `Result<List<AutorResponse>>.Ok(lista)`
- `AutoresController.ListarTodosAsync` trata falha com `this.ErroDe(...)`, o que gera 400 com `ValidationProblemDetails`, e devolve `Ok(result.Valor)` no sucesso.

**Por que (a):**

- **Um modelo de erro só.** Todos os commands e `ListarLivrosQuery` já devolvem `Result`. O erro de validação de autores sai pelo mesmo caminho (`ValidationBehavior` → `TipoErro.Validacao` → `ErroDe` → 400), com o mesmo formato de JSON.
- **Nada muda no behavior nem no `Program.cs`.** A constraint continua garantindo em tempo de compilação que o behavior sabe montar a resposta de falha.
- **Mudança pequena e local:** a query, o handler e um único chamador (o controller).

**O que (a) custa:**

- Um `Result` envolve uma operação que só falha por validação. Quem chama precisa olhar `Sucesso` e `Valor`.
- **O buraco continua aberto para a próxima query.** Se alguém criar um validator para uma query que devolve tipo cru, ele será ignorado do mesmo jeito. A regra passa a ser uma convenção: "query com validator devolve `Result`". Um teste que percorra os `IValidator<T>` do assembly e confira se cada `T` implementa `IRequest<X>` com `X : IFalhaDeValidacao<X>` transformaria essa convenção em verificação. Não implementei esse teste.

**Alternativa descartada:** um segundo behavior, sem a constraint, que lança `ValidationException` quando a resposta não é `Result`, com o `ExceptionHandler` convertendo a exceção em 400. Isso fecharia o buraco para qualquer tipo de resposta, mas criaria dois caminhos de erro de validação (por `Result` e por exceção) com formatos e testes diferentes. Também traria de volta exceção como controle de fluxo, que o projeto acabou de remover com os erros tipados.

## 4. Efeito para quem chama a API

Antes, `GET /api/Autores` nunca devolvia 400: valores fora da faixa eram corrigidos em silêncio pelo handler (`Math.Max` e `Math.Clamp(…, 1, 100)`). Agora o validator vale, e as regras dele são:

| Parâmetro | Antes | Agora |
|---|---|---|
| `pagina = 0` | tratado como página 1 | 400 |
| `tamanhoPagina` de 1 a 100 | 1 a 100 itens | igual (`InclusiveBetween(1, 100)`) |
| `tamanhoPagina = 0` ou `> 100` | corrigido para 1 ou 100 | 400 |

A primeira versão do validator usava `ExclusiveBetween(1, 50)`, que aceitava só de 2 a 49 e contradizia a própria mensagem. A faixa foi corrigida para 1 a 100, a mesma do `Math.Clamp` do handler, então nenhum valor que antes funcionava passa a dar 400.
