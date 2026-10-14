using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Common.Mappings;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterEmprestimoPorId
{
    public class ObterEmprestimoPorIdHandler : IRequestHandler<ObterEmprestimoPorIdQuery, Result<EmprestimoResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ObterEmprestimoPorIdHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<EmprestimoResponse>> Handle(ObterEmprestimoPorIdQuery query, CancellationToken cancellationToken)
        {
            var emprestimo = await _dbContext.Emprestimos
                .AsNoTracking()
                .Where(e => e.Id == query.Id)
                .ProjectToResponse()
                .FirstOrDefaultAsync(cancellationToken);

            if (emprestimo == null)
            {
                return Result<EmprestimoResponse>.Falha($"Empréstimo de id {query.Id} não encontrado");
            }

            return Result<EmprestimoResponse>.Ok(emprestimo);
        }
    }
}