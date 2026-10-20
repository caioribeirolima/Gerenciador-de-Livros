namespace GerenciadorLivro.Application.Common
{
    public class Result<T> : IFalhaDeValidacao<Result<T>>
    {
        public bool Sucesso { get; }
        public T? Valor { get; }
        public string? Erro { get; }
        public TipoErro? Tipo { get; }
        public IReadOnlyDictionary<string, string[]>? ErrosValidacao { get; }

        private Result(bool sucesso, T? valor, string? erro, TipoErro? tipo, IReadOnlyDictionary<string, string[]>? erros)
        {
            Sucesso = sucesso;
            Valor = valor;
            Erro = erro;
            Tipo = tipo;
            ErrosValidacao = erros;
        }

        public static Result<T> Ok(T valor) => new Result<T>(true, valor, null, null, null);
        public static Result<T> Falha(string erro) => new Result<T>(false, default, erro, TipoErro.NaoClassificado, null);
        public static Result<T> NaoEncontrado(string erro) => new Result<T>(false, default, erro, TipoErro.NaoEncontrado, null);
        public static Result<T> Conflito(string erro) => new Result<T>(false, default, erro, TipoErro.Conflito, null);
        public static Result<T> Validacao(IReadOnlyDictionary<string, string[]> erros)
            => new Result<T>(false, default, "Um ou mais erros de validação.", TipoErro.Validacao, erros);
    }


    public class Result : IFalhaDeValidacao<Result>
    {
        public bool Sucesso { get; }
        public string? Erro { get; }
        public TipoErro? Tipo { get; }
        public IReadOnlyDictionary<string, string[]>? ErrosValidacao { get; }

        private Result(bool sucesso, string? erro, TipoErro? tipo, IReadOnlyDictionary<string, string[]>? erros)
        {
            Sucesso = sucesso;
            Erro = erro;
            Tipo = tipo;
            ErrosValidacao = erros;
        }

        public static Result Ok() => new Result(true, null, null, null);
        public static Result Falha(string erro) => new Result(false, erro, TipoErro.NaoClassificado, null);
        public static Result NaoEncontrado(string erro) => new Result(false, erro, TipoErro.NaoEncontrado, null);
        public static Result Conflito(string erro) => new Result(false, erro, TipoErro.Conflito, null);
        public static Result Validacao(IReadOnlyDictionary<string, string[]> erros)
            => new Result(false, "Um ou mais erros de validação.", TipoErro.Validacao, erros);
    }
}
