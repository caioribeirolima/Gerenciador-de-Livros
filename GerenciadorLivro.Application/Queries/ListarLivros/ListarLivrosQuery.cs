using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarLivros
{
    public class ListarLivrosQuery : IRequest<Result<List<LivroResponse>>>
    {
        public string? Autor { get; set; }
        public bool? Disponivel { get; set; }
        public int? Categoria { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
