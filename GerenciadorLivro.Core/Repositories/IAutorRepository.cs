using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IAutorRepository
    {
        Task<List<Autor>> ListarPorIdAsync(List<int> autoresId, CancellationToken cancellationToken);
    }
}
