namespace GerenciadorLivro.Core.Entities
{
    public class Reserva
    {
        public Reserva(int livroId, int usuarioId, DateTime dataReserva)
        {
            LivroId = livroId;
            UsuarioId = usuarioId;
            DataReserva = dataReserva;
            Status = StatusReserva.Ativa;
        }

        public Reserva()
        {
        }

        public int Id { get; set; }
        public int LivroId { get; set; }
        public Livro? Livro { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime DataReserva { get; set; }
        public StatusReserva Status { get; set; } = StatusReserva.Ativa;
        public DateTime? DataNotificacao { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }
}
