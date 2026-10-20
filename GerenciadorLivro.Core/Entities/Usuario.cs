namespace GerenciadorLivro.Core.Entities
{
    public class Usuario
    {

        public Usuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public Usuario(string nome, string email, string cpf) : this(nome, email)
        {
            Cpf = cpf;
        }

        public Usuario()
        {
        }

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Só dígitos (11). Nulo apenas para usuários anteriores ao cadastro com CPF; o cadastro novo sempre exige.
        public string? Cpf { get; set; }
        public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();


    }
}
