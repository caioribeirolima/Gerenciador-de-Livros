using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro
{
    public class AdicionarAutorAoLivroCommand : IRequest<Result>
    {
        public int AutorId { get; set; }
        public int LivroId { get; set; }
    }
}
