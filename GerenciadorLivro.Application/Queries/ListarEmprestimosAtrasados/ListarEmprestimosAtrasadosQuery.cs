using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarEmprestimosAtrasados
{
    public class ListarEmprestimosAtrasadosQuery : IRequest<List<EmprestimoResponse>>
    {
    }
}
