using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.EditarLivro
{
    public class EditarLivroCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int CategoriaId { get; set; }
        public int EditoraId { get; set; }
    }
}
