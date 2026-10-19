using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        void Adicionar(Categoria categoria);
        void EditarAsync(Categoria categoria, CancellationToken cancellationToken);
        void ExcluirAsync(Categoria categoria);
        Task<bool> VerificarDuplicidadePorNome(string nome, CancellationToken cancellationToken);
    }
}
