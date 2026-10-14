using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorHandler : IRequestHandler<CriarAutorCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CriarAutorCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
