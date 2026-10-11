using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarLivroRequest
    {
        [Required]
        [StringLength(200, ErrorMessage = "Máximo de caracteres permitido: 200")]
        public string Titulo { get; set; }
        public int AnoPublicacao { get; set; }
        public int CategoriaId { get; set; }
        public int EditoraId { get; set; }
        public List<int> AutorIds { get; set; } = new List<int>();
    }
}