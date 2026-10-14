using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterAutorPorId
{
    public class ObterAutorPorIdHandler : IRequestHandler<ObterAutorPorIdQuery, Result<AutorResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ObterAutorPorIdHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<AutorResponse>> Handle(ObterAutorPorIdQuery query, CancellationToken cancellationToken)
        {
            var autor = await _dbContext.Autores
                .AsNoTracking()
                .Where(a => a.Id == query.Id)
                .Select(a => new AutorResponse
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    QuantidadeLivros = _dbContext.Livros
                    .Where(l => l.Autores.Any(at => at.Id == a.Id))
                    .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (autor == null)
            {
                return Result<AutorResponse>.Falha($"Autor de id {query.Id} não encontrado");
            }

            return Result<AutorResponse>.Ok(autor);
        }
    }
}
