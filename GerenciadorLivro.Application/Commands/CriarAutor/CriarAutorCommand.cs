using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorCommand : IRequest<Result<int>>
    {
    }
}
