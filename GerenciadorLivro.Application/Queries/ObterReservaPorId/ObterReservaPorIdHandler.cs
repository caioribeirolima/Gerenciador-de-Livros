using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterReservaPorId
{
    public class ObterReservaPorIdHandler : IRequestHandler<ObterReservaPorIdQuery, Result<ReservaResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ObterReservaPorIdHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<ReservaResponse>> Handle(ObterReservaPorIdQuery query, CancellationToken cancellationToken)
        {
            var reserva = await _dbContext.Reservas
                .AsNoTracking()
                .Where(r => r.Id == query.Id)
                .ProjectToResponse(_dbContext.Reservas.AsNoTracking())
                .FirstOrDefaultAsync(cancellationToken);

            if (reserva == null)
            {
                return Result<ReservaResponse>.NaoEncontrado($"Reserva de id {query.Id} não encontrada");
            }

            return Result<ReservaResponse>.Ok(reserva);
        }
    }
}
