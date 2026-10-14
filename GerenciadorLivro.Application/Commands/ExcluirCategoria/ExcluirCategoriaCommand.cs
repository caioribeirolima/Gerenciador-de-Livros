using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ExcluirCategoria
{
    public class ExcluirCategoriaCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
