namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class LivroDetalhesResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public bool Disponivel { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public string EditoraNome { get; set; } = string.Empty;
    }
}
