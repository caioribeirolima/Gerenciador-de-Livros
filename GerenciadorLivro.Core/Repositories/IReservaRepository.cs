using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IReservaRepository
    {
        Task<Reserva?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Reserva?> BuscarReservaPendenteDoUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<Reserva?> BuscarReservaAtivaOrdenadaPorNome(int id, CancellationToken cancellationToken);
        Task<bool> ExisteReservaNotificadaDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteReservaPendenteAsync(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteReservaLivroNotificada(int livroId, CancellationToken cancellationToken);
        void Adicionar(Reserva reserva);
        void RemoverReserva(Reserva reserva);
    }
}
 