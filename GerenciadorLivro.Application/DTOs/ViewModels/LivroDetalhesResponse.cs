namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class LivroDetalhesResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AnoPublicacao { get; set; }
        public bool Disponivel { get; set; }
        public string CategoriaNome { get; set; }
        public string EditoraNome { get; set; }
    }
}
