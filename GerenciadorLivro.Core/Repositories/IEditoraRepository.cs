using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IEditoraRepository
    {
        Task<Editora?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExisteComNomeAsync(string nome, CancellationToken cancellationToken);
        void Adicionar(Editora editora);
        void Excluir(Editora editora);
    }
}
