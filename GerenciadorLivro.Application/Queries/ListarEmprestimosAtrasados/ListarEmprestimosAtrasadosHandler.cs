using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarEmprestimosAtrasados
{
    public class ListarEmprestimosAtrasadosHandler : IRequestHandler<ListarEmprestimosAtrasadosQuery, List<EmprestimoResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarEmprestimosAtrasadosHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<EmprestimoResponse>> Handle(ListarEmprestimosAtrasadosQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Emprestimos
                .AsNoTracking()
                .Where(e => e.DataDevolucaoReal == null && e.DataDevolucaoPrevista < DateTime.Now.Date)
                .ProjectToResponse()
                .ToListAsync(cancellationToken);
        }
    }
}