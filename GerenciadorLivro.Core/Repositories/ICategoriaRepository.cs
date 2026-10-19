using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        void CriarAsync(Categoria categoria, CancellationToken cancellationToken);
        void EditarAsync(Categoria categoria, CancellationToken cancellationToken);
        void ExcluirAsync(Categoria categoria);
        Task<bool> VerificarDuplicidadePorNome(string nome, CancellationToken cancellationToken);
    }
}
