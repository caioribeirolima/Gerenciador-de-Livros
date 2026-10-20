namespace GerenciadorLivro.Core.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<Livro> Livros { get; set; } = new List<Livro>();
    }
}