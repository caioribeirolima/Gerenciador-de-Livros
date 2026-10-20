using Xunit;

namespace GerenciadorLivro.Application.Common
{
    public class ResultTests
    {
        [Fact]
        public void Validacao_GuardaTipoEErrosPorCampo()
        {
            var erros = new Dictionary<string, string[]> { ["Titulo"] = new[] { "obrigatório" } };

            var result = Result<int>.Validacao(erros);

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.Equal("obrigatório", result.ErrosValidacao!["Titulo"][0]);
        }
    }
}
