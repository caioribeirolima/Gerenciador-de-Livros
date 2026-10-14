using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarReservasDoUsuario
{
    public class ListarReservasDoUsuarioHandler : IRequestHandler<ListarReservasDoUsuarioQuery, List<ReservaResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarReservasDoUsuarioHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<ReservaResponse>> Handle(ListarReservasDoUsuarioQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.Reservas
                .AsNoTracking()
                .Where(r => r.UsuarioId == query.UsuarioId)
                .OrderByDescending(r => r.DataReserva)
                .ProjectToResponse(_dbContext.Reservas.AsNoTracking())
                .ToListAsync(cancellationToken);
        }
    }
}
