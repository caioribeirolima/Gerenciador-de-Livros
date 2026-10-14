using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarAutorRequest
    {
        [Required]
        [StringLength(150, ErrorMessage = "O nome do autor deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;
    }
}
