using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _dbContext;
        public ReservaRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Reserva?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Reservas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        public async Task<Reserva?> BuscarReservaPendenteDoUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .FirstOrDefaultAsync(r => r.LivroId == livroId
                    && r.UsuarioId == usuarioId
                    && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

        public void RemoverReserva(Reserva reserva)
            => _dbContext.Reservas.Remove(reserva);

        public void Adicionar(Reserva reserva) => _dbContext.Reservas.Add(reserva);

        public async Task<bool> ExisteReservaPendenteAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId
                    && r.UsuarioId == usuarioId
                    && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

        public async Task<bool> ExisteReservaNotificadaDeOutroUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId
                    && r.Status == StatusReserva.Notificada
                    && r.UsuarioId != usuarioId, cancellationToken);
    }
}
