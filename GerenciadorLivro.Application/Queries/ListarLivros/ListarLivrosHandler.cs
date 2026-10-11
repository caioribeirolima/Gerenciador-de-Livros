using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarLivros
{
    public class ListarLivrosHandler : IRequestHandler<ListarLivrosQuery, Result<List<LivroResponse>>>
    {
        private readonly IAppDbContext _dbContext;

        public ListarLivrosHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<LivroResponse>>> Handle(ListarLivrosQuery request, CancellationToken cancellationToken)
        {
            var pagina = Math.Max(request.Pagina, 1);
            var tamanhoPagina = Math.Clamp(request.TamanhoPagina, 1, 100);

            IQueryable<Livro> query = _dbContext.Livros.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Autor))
                query = query.Where(l => l.Autores.Any(a => a.Nome.Contains(request.Autor)));

            if (request.Disponivel.HasValue)
                query = query.Where(l => l.Disponivel == request.Disponivel.Value);

            if (request.Categoria.HasValue)
                query = query.Where(l => l.CategoriaId == request.Categoria.Value);

            var livros = await query
                .OrderBy(l => l.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
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
