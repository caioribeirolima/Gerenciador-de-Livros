using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Core.Entities
{
    public class Livro
    {

        public Livro(int id, string titulo, int anoPublicacao, bool disponivel, int categoriaId, int editoraId)
        {
            Id = id;
            Titulo = titulo;
            AnoPublicacao = anoPublicacao;
            Disponivel = disponivel;
            CategoriaId = categoriaId;
            EditoraId = editoraId;
        }

        public Livro() { }

        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Máximo de caracteres permitido: 200")]
        public string Titulo { get; set; }
        public int AnoPublicacao { get; set; }
        public bool Disponivel { get; set; } = true;
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        public int EditoraId { get; set; }
        public Editora? Editora { get; set; }
        public bool Excluido { get; set; } = false;
        public List<Autor> Autores { get; set; } = new List<Autor>();
        public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();


    }
}
