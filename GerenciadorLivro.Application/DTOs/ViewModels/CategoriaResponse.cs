namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class CategoriaResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeLivros { get; set; }
    }
}
