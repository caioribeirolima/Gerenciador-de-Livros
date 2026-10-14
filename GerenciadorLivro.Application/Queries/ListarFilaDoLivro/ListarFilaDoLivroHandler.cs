using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarFilaDoLivro
{
    public class ListarFilaDoLivroHandler : IRequestHandler<ListarFilaDoLivroQuery, List<ReservaResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarFilaDoLivroHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<ReservaResponse>> Handle(ListarFilaDoLivroQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.Reservas
                .AsNoTracking()
                .Where(r => r.LivroId == query.LivroId
                         && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada))
                .OrderBy(r => r.Status == StatusReserva.Notificada ? 0 : 1)
                .ThenBy(r => r.DataReserva)
                .ThenBy(r => r.Id)
                .ProjectToResponse(_dbContext.Reservas.AsNoTracking())
                .ToListAsync(cancellationToken);
        }
    }
}
