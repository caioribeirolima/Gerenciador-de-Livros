namespace GerenciadorLivro.Application.Common
{
    public class Result<T>
    {
        public bool Sucesso { get; }
        public T? Valor { get; }
        public string? Erro { get; }

        private Result(bool sucesso, T? valor, string? erro)
        {
            Sucesso = sucesso;
            Valor = valor;
            Erro = erro;
        }

        public static Result<T> Ok(T valor) => new Result<T>(true, valor, null);
        public static Result<T> Falha(string error) => new Result<T>(false, default, error);

    }


    public class Result
    {
        public bool Sucesso { get; }
        public string? Erro { get; }

        private Result(bool sucesso, string? erro)
        {
            Sucesso = sucesso;
            Erro = erro;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Falha(string erro) => new Result(false, erro);
    }
}
