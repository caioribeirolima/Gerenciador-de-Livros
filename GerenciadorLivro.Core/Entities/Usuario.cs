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
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();


    }
}
