namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class EmprestimoResponse
    {
        public int Id { get; set; }
        public string LivroTitulo { get; set; }
        public string UsuarioNome { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
        public DateTime? DataDevolucaoReal { get; set; }
    }
}
