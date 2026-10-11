namespace GerenciadorLivro.Exceptions
{
    public class LivroInvalidoException : Exception
    {
        public LivroInvalidoException(string mensagem) : base(mensagem) { }
    }
}
