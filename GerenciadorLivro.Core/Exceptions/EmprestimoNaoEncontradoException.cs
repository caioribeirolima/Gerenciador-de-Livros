namespace GerenciadorLivro.Exceptions
{
    [Serializable]
    public class EmprestimoNaoEncontradoException : Exception
    {
        public EmprestimoNaoEncontradoException()
        {
        }

        public EmprestimoNaoEncontradoException(string? message) : base(message)
        {
        }

        public EmprestimoNaoEncontradoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}