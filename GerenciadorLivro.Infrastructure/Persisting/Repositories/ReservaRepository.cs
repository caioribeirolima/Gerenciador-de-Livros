using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _dbContext;
        public ReservaRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public void RemoverReserva(Reserva reserva)
        {
            _dbContext.Reservas.Remove(reserva);
        }

        public Reserva Reservar(int livroId, int usuarioId, CancellationToken cancellationToken)
        {
            var reserva = new Reserva(livroId, usuarioId, DateTime.Now);
            _dbContext.Reservas.AddAsync(reserva, cancellationToken);

            return reserva;
        }

        public async Task<bool> VerificarReservaDuplicada(int livroId, int usuarioId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId
                && r.UsuarioId == usuarioId
                && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

            return result;
        }

        public async Task<bool> VerificarReservaOutroUsuario(int LivroId, int UsuarioId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == LivroId
                && r.Status == StatusReserva.Notificada
                && r.UsuarioId != UsuarioId, cancellationToken);

            return result;
        }
    }
}
