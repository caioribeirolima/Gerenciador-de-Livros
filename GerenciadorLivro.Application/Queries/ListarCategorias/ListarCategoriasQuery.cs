using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarCategorias
{
    public class ListarCategoriasQuery : IRequest<List<CategoriaResponse>>
    {
        public string? Nome { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
