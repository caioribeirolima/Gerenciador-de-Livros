using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ExcluirLivro
{
    public class ExcluirLivroCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
