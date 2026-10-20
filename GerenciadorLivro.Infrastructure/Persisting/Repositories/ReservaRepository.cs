using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _dbContext;
        public ReservaRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Reserva?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Reservas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        private static Expression<Func<Reserva, bool>> PendenteDoUsuario(int livroId, int usuarioId)
            => r => r.LivroId == livroId
                && r.UsuarioId == usuarioId
                && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada);

        public async Task<Reserva?> BuscarReservaPendenteDoUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .FirstOrDefaultAsync(PendenteDoUsuario(livroId, usuarioId), cancellationToken);

        public void RemoverReserva(Reserva reserva)
            => _dbContext.Reservas.Remove(reserva);

        public void Adicionar(Reserva reserva) => _dbContext.Reservas.Add(reserva);

        public async Task<bool> ExisteReservaPendenteAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .AnyAsync(PendenteDoUsuario(livroId, usuarioId), cancellationToken);

        public async Task<bool> ExisteReservaPendenteDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId
                    && r.UsuarioId != usuarioId
                    && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

        public async Task<bool> ExisteReservaNotificadaDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId
                    && r.Status == StatusReserva.Notificada
                    && r.UsuarioId != usuarioId, cancellationToken);

        public async Task<bool> ExisteReservaLivroNotificada(int livroId, CancellationToken cancellationToken)
        {
            var jaExisteNotificada = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId && r.Status == StatusReserva.Notificada, cancellationToken);

            return jaExisteNotificada;
        }


        public async Task<Reserva?> BuscarReservaAtivaOrdenadaPorNome(int id, CancellationToken cancellationToken)
        {
            var proxima = await _dbContext.Reservas
                .Where(r => r.LivroId == id && r.Status == StatusReserva.Ativa)
                .OrderBy(r => r.DataReserva)
                .ThenBy(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return proxima;
        }
    }
}
