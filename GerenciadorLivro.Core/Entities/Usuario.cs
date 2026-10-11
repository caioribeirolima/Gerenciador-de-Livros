namespace GerenciadorLivro.Core.Entities
{
    public class Usuario
    {

        public Usuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public Usuario()
        {
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();


    }
}
