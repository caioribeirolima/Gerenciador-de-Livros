using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IEditoraRepository
    {
        Task<Editora?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Editora?> BuscarPorNomeAsync(int id, CancellationToken cancellationToken);
        Task<bool> VerificarSeExiste(string nome, CancellationToken cancellationToken);
        void AdicionarAsync(Editora editora, CancellationToken cancellationToken);
        void ExcluirAsync(Editora editora);
    }
}
