# Nota de design: testar o validator direto ou via IMediator

Os dois tipos de teste existem para `CriarLivroCommand`:

- **Direto no validator:** `CriarLivroCommandValidatorTests`. Usa `TestValidate` do `FluentValidation.TestHelper` e instancia `new CriarLivroCommandValidator(new RelogioFixo())`, sem MediatR, sem DI e sem banco. São 11 casos: o comando válido e os limites de título (200/201), ano (atual/atual + 1), `CategoriaId`, `EditoraId` e item de `AutorIds` (0/1).
- **Via IMediator:** os testes `CriarLivro_*` de `LivrosTests`. Montam o container do `BancoDeTeste` e mandam o comando pelo pipeline completo, com banco em memória.

## O que o teste direto prova e o via IMediator não prova

1. **O limite exato de cada regra.** Cada caso muda um campo de um comando válido e confere só aquela propriedade (`ShouldHaveValidationErrorFor(x => x.Titulo)`). Isso diferencia `MaximumLength(200)` de `MaximumLength(199)` e `GreaterThan(0)` de `GreaterThanOrEqualTo(0)`. Os testes via IMediator usam valores claramente inválidos (título vazio, ano + 1 pelo relógio real) e não exercitam a fronteira.
2. **Que a regra do ano usa o `TimeProvider`, e não o relógio da máquina.** O `RelogioFixo` está em 2030. Se o validator voltasse a usar `DateTime.Now.Year`, `AnoPublicacao_IgualAoAnoAtual_Valido` falharia, porque 2030 é futuro no relógio real. Via IMediator o `BancoDeTeste` registra `TimeProvider.System`, então os dois relógios coincidem e uma regressão dessas passaria.
3. **Que cada regra funciona isolada.** Uma falha aponta para uma regra só. No IMediator, um erro pode vir do validator, do behavior ou do handler.

## O que o teste via IMediator prova e o direto não prova

1. **Que o validator é executado de fato.** O teste direto chama o validator na mão e passaria mesmo se ninguém o chamasse em produção. Foi exatamente o que aconteceu com o `ListarAutoresQueryValidator` (ver `NOTA_DE_DESIGN_VALIDACAO_LISTAR_AUTORES.md`): o validator estava certo, mas o `ValidationBehavior` era pulado pela constraint de `TResponse`. Só o teste via IMediator pega isso.
2. **Que o validator é resolvido pelo container.** Agora o `CriarLivroCommandValidator` depende de `TimeProvider`. Se ninguém o registrar, o `AddValidatorsFromAssembly` não consegue construir o validator e o request quebra em tempo de execução. O teste direto faz o `new` na mão e não percebe.
3. **O contrato que o controller recebe.** Que as falhas viram `Result` com `TipoErro.Validacao`, que as chaves de `ErrosValidacao` são as esperadas (`AutorIds[1]`, agrupadas pelo behavior) e que o handler não roda, sem nenhum livro gravado.

## O que nenhum dos dois prova

O teste via IMediator usa o container do `BancoDeTeste`, não o do `Program.cs`. Se alguém remover `AddSingleton(TimeProvider.System)` do `Program.cs`, os 94 testes continuam verdes e a API quebra no primeiro `POST /api/Livros`. Cobrir isso exige um teste de integração com `WebApplicationFactory<Program>`, que o projeto ainda não tem.
