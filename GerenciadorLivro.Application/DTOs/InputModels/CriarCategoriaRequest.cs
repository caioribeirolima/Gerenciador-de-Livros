using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarCategoriaRequest
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
    }
}
