using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarEmprestimosAtivos
{
    public class ListarEmprestimosAtivosQuery : IRequest<List<EmprestimoResponse>>
    {
    }
}
