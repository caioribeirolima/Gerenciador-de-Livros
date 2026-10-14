using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarEditora
{
    public class CriarEditoraCommand : IRequest<Result<int>>
    {
        public string Nome { get; set; } = string.Empty;
    }
}
