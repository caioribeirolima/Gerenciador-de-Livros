using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.RenovarEmprestimo
{
    public class RenovarEmprestimoCommand : IRequest<Result<int>>
    {
        public int EmprestimoId { get; set; }
    }
}
