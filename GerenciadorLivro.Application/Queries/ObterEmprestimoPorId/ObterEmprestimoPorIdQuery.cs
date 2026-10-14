using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterEmprestimoPorId
{
    public class ObterEmprestimoPorIdQuery : IRequest<Result<EmprestimoResponse>>
    {
        public int Id { get; set; }
    }
}
