using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroCommand : IRequest<Result<int>>
    {
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int CategoriaId { get; set; }
        public int EditoraId { get; set; }
        public List<int> AutorIds { get; set; } = new();
    }
}
