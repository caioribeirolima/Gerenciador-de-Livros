using FluentValidation.TestHelper;
using GerenciadorLivro.Application.Commands.CriarLivro;
using Xunit;

namespace GerenciadorLivro.Tests.Livros
{
    public class CriarLivroCommandValidatorTests
    {
        private const int AnoAtual = 2030;

        private class RelogioFixo : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => new DateTimeOffset(AnoAtual, 6, 15, 12, 0, 0, TimeSpan.Zero);
            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }

        private readonly CriarLivroCommandValidator _validator = new(new RelogioFixo());

        private static CriarLivroCommand ComandoValido() => new()
        {
            Titulo = "Livro",
            AnoPublicacao = 2020,
            CategoriaId = 1,
            EditoraId = 1,
            AutorIds = new List<int> { 1 }
        };

        [Fact]
        public void ComandoValido_NaoTemErros()
        {
            _validator.TestValidate(ComandoValido()).ShouldNotHaveAnyValidationErrors();
        }

        // ---------- Titulo ----------

        [Fact]
        public void Titulo_Com200Caracteres_Valido()
        {
            var command = ComandoValido();
            command.Titulo = new string('a', 200);

            _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Titulo);
        }

        [Fact]
        public void Titulo_Com201Caracteres_Invalido()
        {
            var command = ComandoValido();
            command.Titulo = new string('a', 201);

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Titulo);
        }

        // ---------- AnoPublicacao ----------

        [Fact]
        public void AnoPublicacao_IgualAoAnoAtual_Valido()
        {
            var command = ComandoValido();
            command.AnoPublicacao = AnoAtual;

            _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.AnoPublicacao);
        }

        [Fact]
        public void AnoPublicacao_AnoAtualMaisUm_Invalido()
        {
            var command = ComandoValido();
            command.AnoPublicacao = AnoAtual + 1;

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.AnoPublicacao);
        }

        // ---------- Ids ----------

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void CategoriaId_Limite(int categoriaId, bool temErro)
        {
            var command = ComandoValido();
            command.CategoriaId = categoriaId;

            var resultado = _validator.TestValidate(command);

            if (temErro) resultado.ShouldHaveValidationErrorFor(x => x.CategoriaId);
            else resultado.ShouldNotHaveValidationErrorFor(x => x.CategoriaId);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void EditoraId_Limite(int editoraId, bool temErro)
        {
            var command = ComandoValido();
            command.EditoraId = editoraId;

            var resultado = _validator.TestValidate(command);

            if (temErro) resultado.ShouldHaveValidationErrorFor(x => x.EditoraId);
            else resultado.ShouldNotHaveValidationErrorFor(x => x.EditoraId);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void AutorId_Limite(int autorId, bool temErro)
        {
            var command = ComandoValido();
            command.AutorIds = new List<int> { autorId };

            var resultado = _validator.TestValidate(command);

            if (temErro) resultado.ShouldHaveValidationErrorFor("AutorIds[0]");
            else resultado.ShouldNotHaveValidationErrorFor("AutorIds[0]");
        }
    }
}
