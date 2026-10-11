namespace GerenciadorLivro.Exceptions
{
    [Serializable]
    public class EmprestimosNaoEncontradosException : Exception
    {
        public EmprestimosNaoEncontradosException()
        {
        }

        public EmprestimosNaoEncontradosException(string? message) : base(message)
        {
        }

        public EmprestimosNaoEncontradosException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}