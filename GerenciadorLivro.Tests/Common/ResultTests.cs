using GerenciadorLivro.Application.Common;
using Xunit;

namespace GerenciadorLivro.Tests.Common
{
    public class ResultTests
    {
        private static T Falhar<T>() where T : IFalhaDeValidacao<T>
            => T.Validacao(new Dictionary<string, string[]>());

        [Fact]
        public void Validacao_GuardaTipoEErrosPorCampo()
        {
            var erros = new Dictionary<string, string[]> { ["Titulo"] = new[] { "obrigatório" } };

            var result = Result<int>.Validacao(erros);

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.Equal("obrigatório", result.ErrosValidacao!["Titulo"][0]);
        }

        [Fact]
        public void FalhaDeValidacao_FuncionaParaResultComEPara_ResultSemGenerico()
        {
            Assert.Equal(TipoErro.Validacao, Falhar<Result<int>>().Tipo);
            Assert.Equal(TipoErro.Validacao, Falhar<Result>().Tipo);
        }
    }
}
