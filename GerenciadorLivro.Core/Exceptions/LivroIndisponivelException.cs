namespace GerenciadorLivro.Exceptions
{
    [Serializable]
    public class LivroIndisponivelException : Exception
    {
        public LivroIndisponivelException()
        {
        }

        public LivroIndisponivelException(string? message) : base(message)
        {
        }

        public LivroIndisponivelException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}