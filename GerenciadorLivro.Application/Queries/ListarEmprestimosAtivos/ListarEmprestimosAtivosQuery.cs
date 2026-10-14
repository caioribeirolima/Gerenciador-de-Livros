using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarEmprestimosAtivos
{
    public class ListarEmprestimosAtivosQuery : IRequest<List<EmprestimoResponse>>
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
