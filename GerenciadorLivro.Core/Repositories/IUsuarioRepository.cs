namespace GerenciadorLivro.Core.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteAsync(int id, CancellationToken cancellationToken);
    }
}
