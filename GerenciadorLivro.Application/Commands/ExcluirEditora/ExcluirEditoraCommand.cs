using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ExcluirEditora
{
    public class ExcluirEditoraCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
