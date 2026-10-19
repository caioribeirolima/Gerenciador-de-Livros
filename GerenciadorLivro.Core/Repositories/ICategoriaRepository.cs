using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        void Adicionar(Categoria categoria);
        void ExcluirAsync(Categoria categoria);
        Task<bool> ExisteComNomeAsync(string nome, CancellationToken cancellationToken);
    }
}
