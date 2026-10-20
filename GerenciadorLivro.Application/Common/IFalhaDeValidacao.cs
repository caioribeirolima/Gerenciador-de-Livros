namespace GerenciadorLivro.Application.Common
{
    public interface IFalhaDeValidacao<TSelf> where TSelf : IFalhaDeValidacao<TSelf>
    {
        static abstract TSelf Validacao(IReadOnlyDictionary<string, string[]> erros);
    }
}