using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IAutorRepository
    {
        Task<List<Autor>> ListarPorIdAsync(List<int> autoresId, CancellationToken cancellationToken);
        Task<Autor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        void Adicionar(Autor autor);
        Task<bool> ExisteComNomeAsync(string nome, CancellationToken cancellationToken);
    }
}
