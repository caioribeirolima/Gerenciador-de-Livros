using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IReservaRepository
    {
        Task<Reserva?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Reserva?> BuscarReservaPendenteDoUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<Reserva?> BuscarPrimeiraReservaAtivaDoLivroAsync(int livroId, CancellationToken cancellationToken);
        Task<bool> ExisteReservaNotificadaDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteOutraReservaNotificadaDoLivroAsync(int livroId, int reservaIdIgnorada, CancellationToken cancellationToken);
        Task<bool> ExisteReservaPendenteDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteReservaPendenteAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteReservaNotificadaDoLivroAsync(int livroId, CancellationToken cancellationToken);
        void Adicionar(Reserva reserva);
        void RemoverReserva(Reserva reserva);
    }
}
 