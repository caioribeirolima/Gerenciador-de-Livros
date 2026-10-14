using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ReservarLivro
{
    public class ReservarLivroCommand : IRequest<Result<int>>
    {
        public int LivroId { get; set; }
        public int UsuarioId { get; set; }
    }
}
