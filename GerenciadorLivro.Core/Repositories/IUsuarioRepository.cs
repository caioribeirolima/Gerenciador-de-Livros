using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteAsync(int id, CancellationToken cancellationToken);
        Task<Usuario?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);

        // cpf já normalizado (só dígitos).
        Task<bool> ExisteComCpfAsync(string cpf, CancellationToken cancellationToken);
        void Adicionar(Usuario usuario);
        void Remover(Usuario usuario);
    }
}
