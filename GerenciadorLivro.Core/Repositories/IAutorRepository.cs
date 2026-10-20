using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IAutorRepository
    {
        Task<List<Autor>> ListarPorIdsAsync(IEnumerable<int> autoresId, CancellationToken cancellationToken);
        Task<Autor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        void Adicionar(Autor autor);
        Task<bool> ExisteComNomeAsync(string nome, CancellationToken cancellationToken);
    }
}
