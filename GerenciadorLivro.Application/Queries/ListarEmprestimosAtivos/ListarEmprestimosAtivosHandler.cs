using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarEmprestimosAtivos
{
    public class ListarEmprestimosAtivosHandler : IRequestHandler<ListarEmprestimosAtivosQuery, List<EmprestimoResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarEmprestimosAtivosHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<EmprestimoResponse>> Handle(ListarEmprestimosAtivosQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Emprestimos
                .AsNoTracking()
                .Where(e => e.DataDevolucaoReal == null)
                .ProjectToResponse()
                .ToListAsync(cancellationToken);
        }
    }
}