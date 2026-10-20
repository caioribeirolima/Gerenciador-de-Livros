using FluentValidation.TestHelper;
using GerenciadorLivro.Application.Commands.CriarUsuario;
using Xunit;

namespace GerenciadorLivro.Tests.Usuarios
{
    public class CriarUsuarioCommandValidatorTests
    {
        private readonly CriarUsuarioCommandValidator _validator = new();

        private static CriarUsuarioCommand ComandoValido() => new()
        {
            Nome = "Maria da Silva",
            Email = "maria@hospital.com.br",
            Cpf = "529.982.247-25"
        };

        [Fact]
        public void ComandoValido_NaoTemErros()
        {
            _validator.TestValidate(ComandoValido()).ShouldNotHaveAnyValidationErrors();
        }

        // ---------- Nome ----------

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Nome_Vazio_Invalido(string nome)
        {
            var command = ComandoValido();
            command.Nome = nome;

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void Nome_Com150Caracteres_Valido()
        {
            var command = ComandoValido();
            command.Nome = new string('a', 150);

            _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void Nome_Com151Caracteres_Invalido()
        {
            var command = ComandoValido();
            command.Nome = new string('a', 151);

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Nome);
        }

        // ---------- Email ----------

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("maria")]
        [InlineData("maria@")]
        [InlineData("@hospital.com.br")]
        public void Email_VazioOuMalFormado_Invalido(string email)
        {
            var command = ComandoValido();
            command.Email = email;

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_Com254Caracteres_Valido()
        {
            var command = ComandoValido();
            command.Email = new string('a', 254 - "@x.com".Length) + "@x.com";

            _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_Com255Caracteres_Invalido()
        {
            var command = ComandoValido();
            command.Email = new string('a', 255 - "@x.com".Length) + "@x.com";

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Email);
        }

        // ---------- Cpf ----------

        [Theory]
        [InlineData("52998224725")]
        [InlineData("529.982.247-25")]
        public void Cpf_ComOuSemMascara_Valido(string cpf)
        {
            var command = ComandoValido();
            command.Cpf = cpf;

            _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Cpf);
        }

        [Theory]
        [InlineData("5299822472")]
        [InlineData("529982247255")]
        [InlineData("11111111111")]
        [InlineData("52998224726")]
        [InlineData("529.982.247-2a")]
        public void Cpf_Invalido(string cpf)
        {
            var command = ComandoValido();
            command.Cpf = cpf;

            _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Cpf);
        }

        [Fact]
        public void Cpf_Vazio_ReportaUmaUnicaMensagem()
        {
            var command = ComandoValido();
            command.Cpf = "";

            var resultado = _validator.TestValidate(command);

            var erros = resultado.Errors.Where(e => e.PropertyName == nameof(CriarUsuarioCommand.Cpf)).ToList();
            Assert.Single(erros);
            Assert.Equal("O CPF é obrigatório.", erros[0].ErrorMessage);
        }

        [Fact]
        public void Cpf_Invalido_MensagemNaoRepeteOValorInformado()
        {
            var command = ComandoValido();
            command.Cpf = "52998224726";

            var resultado = _validator.TestValidate(command);

            var mensagem = resultado.Errors.Single(e => e.PropertyName == nameof(CriarUsuarioCommand.Cpf)).ErrorMessage;
            Assert.DoesNotContain("52998224726", mensagem);
        }
    }
}
