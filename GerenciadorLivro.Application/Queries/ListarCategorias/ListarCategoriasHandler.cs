using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarCategorias
{
    public class ListarCategoriasHandler : IRequestHandler<ListarCategoriasQuery, List<CategoriaResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarCategoriasHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<CategoriaResponse>> Handle(ListarCategoriasQuery request, CancellationToken cancellationToken)
        {
            var pagina = Math.Max(request.Pagina, 1);
            var tamanhoPagina = Math.Clamp(request.TamanhoPagina, 1, 100);

            IQueryable<Categoria> query = _dbContext.Categorias.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Nome))
                query = query.Where(c => c.Nome.Contains(request.Nome));

            var categorias = await query
                .OrderBy(c => c.Nome)
                .ThenBy(c => c.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(c => new CategoriaResponse
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    QuantidadeLivros = _dbContext.Livros.Count(l => l.CategoriaId == c.Id)
                })
                .ToListAsync(cancellationToken);

            return categorias;
        }
    }
}
