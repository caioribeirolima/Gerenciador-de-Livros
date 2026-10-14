using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class AutorResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public List<Livro> QuantidadeLivros { get; set; } = new List<Livro>();
    }
}
