namespace GerenciadorLivro.Application.Common
{
    public class Result<T>
    {
        public bool Sucesso { get; }
        public T? Valor { get; }
        public string? Erro { get; }
        public TipoErro? Tipo { get; }            // null quando deu certo

        private Result(bool sucesso, T? valor, string? erro, TipoErro? tipo)
        {
            Sucesso = sucesso;
            Valor = valor;
            Erro = erro;
            Tipo = tipo;
        }

        public static Result<T> Ok(T valor) => new Result<T>(true, valor, null, null);
        public static Result<T> Falha(string erro) => new Result<T>(false, default, erro, TipoErro.NaoClassificado);
        public static Result<T> NaoEncontrado(string erro) => new Result<T>(false, default, erro, TipoErro.NaoEncontrado);
        public static Result<T> Conflito(string erro) => new Result<T>(false, default, erro, TipoErro.Conflito);
    }


    public class Result
    {
        public bool Sucesso { get; }
        public string? Erro { get; }
        public TipoErro? Tipo { get; }          

        private Result(bool sucesso, string? erro, TipoErro? tipo)
        {
            Sucesso = sucesso;
            Erro = erro;
            Tipo = tipo;
        }

        public static Result Ok() => new Result(true, null, null);
        public static Result Falha(string erro) => new Result(false, erro, TipoErro.NaoClassificado);
        public static Result NaoEncontrado(string erro) => new Result(false, erro, TipoErro.NaoEncontrado);
        public static Result Conflito(string erro) => new Result(false, erro, TipoErro.Conflito);
    }
}
