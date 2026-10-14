using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarAutores
{
    public class ListarAutoresHandler : IRequestHandler<ListarAutoresQuery, List<AutorResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarAutoresHandler(IAppDbContext dbContext) => _dbContext = dbContext;


        public async Task<List<AutorResponse>> Handle(ListarAutoresQuery query, CancellationToken cancellationToken)
        {
            var pagina = Math.Max(query.Pagina, 1);
            var tamanhoPagina = Math.Clamp(query.TamanhoPagina, 1, 100);

            IQueryable<Autor> autores = _dbContext.Autores.AsNoTracking();

            if(!string.IsNullOrWhiteSpace(query.Nome))
            {
                autores = autores.Where(a => a.Nome.Contains(query.Nome));
            }

            var lista = await autores
                .OrderBy(a => a.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(a => new AutorResponse
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    QuantidadeLivros = _dbContext.Livros
                    .Where(l => l.Autores.Any(at => at.Id == a.Id))
                    .ToList()
                })
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}
