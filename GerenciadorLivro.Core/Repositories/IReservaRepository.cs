using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IReservaRepository
    {
        Task<bool> VerificarReservaOutroUsuario(int LivroId, int UsuarioId, CancellationToken cancellationToken);
        Task<bool> VerificarReservaDuplicada(int LivroId, int UsuarioId, CancellationToken cancellationToken);
        Reserva Reservar(int livroId, int usuarioId, CancellationToken cancellationToken);
        void RemoverReserva(Reserva reserva);
    }
}
 