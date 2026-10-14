using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.DevolverEmprestimo
{
    public class DevolverEmprestimoCommand : IRequest<Result>
    {
        public int EmprestimoId { get; set; }
    }
}
