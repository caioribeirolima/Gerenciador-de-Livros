namespace GerenciadorLivro.Core.Entities
{
    public class Emprestimo
    {
        public const int LimiteRenovacoes = 2;

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
        public int Renovacoes { get; set; }

        // Regras que dependem só do próprio empréstimo; as que precisam do banco ficam no handler.
        public string? MotivoParaNaoRenovar(DateTime hoje)
        {
            if (DataDevolucaoReal != null)
                return "Empréstimo já devolvido.";

            if (Renovacoes >= LimiteRenovacoes)
                return "Empréstimo já atingiu o limite de renovações.";

            if (DataDevolucaoPrevista.Date < hoje.Date)
                return "Empréstimo está atrasado e não pode ser renovado.";

            return null;
        }

        public void Renovar(int diasParaDevolucao, DateTime hoje)
        {
            var motivo = MotivoParaNaoRenovar(hoje);

            if (motivo != null)
                throw new InvalidOperationException(motivo);

            DataDevolucaoPrevista = DataDevolucaoPrevista.AddDays(diasParaDevolucao);
            Renovacoes++;
        }
    }
}
