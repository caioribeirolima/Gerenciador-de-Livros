using GerenciadorLivro.Application.Common;
using Xunit;

namespace GerenciadorLivro.Tests.Common
{
    public class CpfTests
    {
        [Theory]
        [InlineData("52998224725")]
        [InlineData("529.982.247-25")]
        [InlineData("11144477735")]
        [InlineData("111.444.777-35")]
        [InlineData("390.533.447-05")]
        [InlineData("123.456.789-09")]
        public void EhValido_CpfCorreto_ComOuSemMascara(string cpf)
        {
            Assert.True(Cpf.EhValido(cpf));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("5299822472")]           // 10 dígitos
        [InlineData("529982247255")]         // 12 dígitos
        [InlineData("52998224726")]          // 2º dígito verificador errado
        [InlineData("52998224735")]          // 1º dígito verificador errado
        [InlineData("11111111111")]          // sequência repetida passa na conta, mas não é CPF
        [InlineData("000.000.000-00")]
        [InlineData("529.982.247-2a")]       // letra
        [InlineData("529-982-247.25")]       // pontuação fora do padrão
        [InlineData("529.982.247-25\n")]     // quebra de linha no fim
        [InlineData("٥٢٩٩٨٢٢٤٧٢٥")]          // dígitos de outro alfabeto (Unicode), não ASCII
        public void EhValido_CpfIncorreto(string? cpf)
        {
            Assert.False(Cpf.EhValido(cpf));
        }

        [Theory]
        [InlineData("529.982.247-25", "52998224725")]
        [InlineData("52998224725", "52998224725")]
        [InlineData(" 529.982.247-25 ", "52998224725")]
        [InlineData(null, "")]
        public void Normalizar_MantemSoOsDigitos(string? entrada, string esperado)
        {
            Assert.Equal(esperado, Cpf.Normalizar(entrada));
        }

        [Fact]
        public void Mascarar_EscondeInicioEFim()
        {
            Assert.Equal("***.982.247-**", Cpf.Mascarar("52998224725"));
            Assert.Equal("***.982.247-**", Cpf.Mascarar("529.982.247-25"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("123")]
        public void Mascarar_SemOnzeDigitos_DevolveNull(string? cpf)
        {
            Assert.Null(Cpf.Mascarar(cpf));
        }
    }
}
