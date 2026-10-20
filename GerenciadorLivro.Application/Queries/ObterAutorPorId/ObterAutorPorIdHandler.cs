using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterAutorPorId
{
    public class ObterAutorPorIdHandler : IRequestHandler<ObterAutorPorIdQuery, Result<AutorDetalhesResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ObterAutorPorIdHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<AutorDetalhesResponse>> Handle(ObterAutorPorIdQuery query, CancellationToken cancellationToken)
        {
            var autor = await _dbContext.Autores
                .AsNoTracking()
                .Where(a => a.Id == query.Id)
                .Select(a => new AutorDetalhesResponse
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Livros = a.Livros
                        .Select(l => new LivroResumoResponse { Id = l.Id, Titulo = l.Titulo })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (autor == null)
            {
                return Result<AutorDetalhesResponse>.NaoEncontrado($"Autor de id {query.Id} não encontrado");
            }

            return Result<AutorDetalhesResponse>.Ok(autor);
        }
    }
}
