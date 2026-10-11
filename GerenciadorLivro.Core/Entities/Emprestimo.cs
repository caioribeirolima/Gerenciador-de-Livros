namespace GerenciadorLivro.Core.Entities
{
    public class Emprestimo
    {
        public Emprestimo(
            int id, 
            int livroId, 
            int usuarioId, 
            DateTime dataEmprestimo, 
            DateTime dataDevolucaoPrevista
            )
        {
            Id = id;
            LivroId = livroId;
            UsuarioId = usuarioId;
            DataEmprestimo = dataEmprestimo;
            DataDevolucaoPrevista = dataDevolucaoPrevista;
        }

        public Emprestimo()
        {
        }

        public int Id { get; set; }
        public int LivroId { get; set; }
        public Livro? Livro { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
        public DateTime? DataDevolucaoReal { get; set; }
    }
}
