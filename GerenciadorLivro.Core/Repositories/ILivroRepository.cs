using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface ILivroRepository
    {
        Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Livro?> BuscarPorIdIncluindoExcluidosAsync(int id, CancellationToken cancellationToken);
    }
}