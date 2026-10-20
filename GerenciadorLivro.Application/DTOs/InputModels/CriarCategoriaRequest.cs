using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
    }
}
