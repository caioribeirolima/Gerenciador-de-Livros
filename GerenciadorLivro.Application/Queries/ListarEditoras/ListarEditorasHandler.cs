using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ListarEditoras
{
    public class ListarEditorasHandler : IRequestHandler<ListarEditorasQuery, List<EditorasResponse>>
    {
        private readonly IAppDbContext _dbContext;
        public ListarEditorasHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<EditorasResponse>> Handle(ListarEditorasQuery query, CancellationToken cancellationToken)
        {
            var pagina = Math.Max(query.Pagina, 1);
            var tamanhoPagina = Math.Clamp(query.TamanhoPagina, 1, 100);

            IQueryable<Editora> editoras = _dbContext.Editoras.AsNoTracking();

            var lista = await editoras
                .OrderBy(e => e.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(e => new EditorasResponse
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    QuantidadeLivros = e.Livros.Count
                })
                .ToListAsync(cancellationToken);

            return lista;
        }
    }
}
