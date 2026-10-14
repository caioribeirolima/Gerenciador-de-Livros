using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.EditarCategoria
{
    public class EditarCategoriaCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
