namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class AutorDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<LivroResumoResponse> Livros { get; set; } = new();
    }
}
