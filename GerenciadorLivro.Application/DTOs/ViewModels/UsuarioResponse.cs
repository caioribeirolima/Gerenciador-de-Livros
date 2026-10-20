namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Mascarado (***.982.247-**): a API nunca devolve o CPF inteiro. Nulo para usuário antigo sem CPF.
        public string? CpfMascarado { get; set; }
    }
}
