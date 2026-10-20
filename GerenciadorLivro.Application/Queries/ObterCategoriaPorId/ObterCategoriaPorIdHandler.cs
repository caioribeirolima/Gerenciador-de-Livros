using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterCategoriaPorId
{
    public class ObterCategoriaPorIdHandler : IRequestHandler<ObterCategoriaPorIdQuery, Result<CategoriaResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ObterCategoriaPorIdHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<CategoriaResponse>> Handle(ObterCategoriaPorIdQuery query, CancellationToken cancellationToken)
        {
            var categoria = await _dbContext.Categorias
                .AsNoTracking()
                .Where(c => c.Id == query.Id)
                .Select(c => new CategoriaResponse
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    QuantidadeLivros = _dbContext.Livros.Count(l => l.CategoriaId == c.Id)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (categoria == null)
            {
                return Result<CategoriaResponse>.NaoEncontrado("Categoria inexistente");
            }

            return Result<CategoriaResponse>.Ok(categoria);
        }
    }
}
