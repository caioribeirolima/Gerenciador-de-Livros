using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaCommand : IRequest<Result<int>>
    {
        public string Nome { get; set; } = string.Empty;
    }
}
