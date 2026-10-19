using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IReservaRepository
    {
        Task<Reserva?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Reserva?> BuscarReservaAtivaPorUsuarioELivro(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> VerificarReservaOutroUsuario(int livroId, int usuarioId, CancellationToken cancellationToken);
        Task<bool> VerificarReservaDuplicada(int livroId, int usuarioId, CancellationToken cancellationToken);
        Reserva Reservar(int livroId, int usuarioId, CancellationToken cancellationToken);
        void RemoverReserva(Reserva reserva);
    }
}
 