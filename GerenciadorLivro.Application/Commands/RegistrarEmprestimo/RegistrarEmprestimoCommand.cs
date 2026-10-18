using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.RegistrarEmprestimo
{
    public class RegistrarEmprestimoCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int LivroId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
    }
}
