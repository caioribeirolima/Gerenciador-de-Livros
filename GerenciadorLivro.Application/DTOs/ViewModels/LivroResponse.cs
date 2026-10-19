using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class LivroResponse
    {

        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public bool Disponivel { get; set; } = true;
        public List<AutorResponse> Autores { get; set; } = new();
    }
}
