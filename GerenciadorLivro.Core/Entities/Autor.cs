using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Core.Entities
{
    public class Autor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "O nome do autor deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        public List<Livro> Livros { get; set; } = new List<Livro>();
    }
}