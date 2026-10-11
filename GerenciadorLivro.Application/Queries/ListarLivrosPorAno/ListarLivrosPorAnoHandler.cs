using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarLivrosPorAno
{
    public class ListarLivrosPorAnoHandler : IRequestHandler<ListarLivrosPorAnoQuery, Result<List<LivroResponse>>>
    {
        private readonly IAppDbContext _dbContext;

        public ListarLivrosPorAnoHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<List<LivroResponse>>> Handle(ListarLivrosPorAnoQuery request, CancellationToken cancellationToken)
        {
            var livros = await _dbContext.Livros
                .AsNoTracking()
                .Where(l => l.AnoPublicacao == request.Ano)
                .Select(l => new LivroResponse
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    AnoPublicacao = l.AnoPublicacao,
                    Disponivel = l.Disponivel
                })
                .ToListAsync(cancellationToken);

            return Result<List<LivroResponse>>.Ok(livros);
        }
    }
}
