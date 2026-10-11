using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarEditoraRequest
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
    }
}
