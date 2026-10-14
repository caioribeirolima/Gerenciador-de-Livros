namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarReservaRequest
    {
        public int LivroId { get; set; }
        public int UsuarioId { get; set; }
    }
}
