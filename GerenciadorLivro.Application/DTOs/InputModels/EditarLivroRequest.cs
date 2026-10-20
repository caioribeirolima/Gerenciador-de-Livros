using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class EditarLivroRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int CategoriaId { get; set; }
        public int EditoraId { get; set; }
    }
}
