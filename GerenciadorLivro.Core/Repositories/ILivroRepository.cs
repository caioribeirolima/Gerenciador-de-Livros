using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface ILivroRepository
    {
        void Adicionar(Livro livro);
        Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Livro?> BuscarPorIdIncluindoExcluidosAsync(int id, CancellationToken cancellationToken);
        Task<Livro?> BuscarPorIdIncluindoAutoresAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExisteLivroNaCategoriaAsync(int categoriaId, CancellationToken cancellationToken);
        Task<bool> ExisteLivroNaEditoraAsync(int id, CancellationToken cancellationToken);
        Task<bool> VerificarLivroDisponivel(int id, CancellationToken cancellationToken);
        void Excluir(Livro livro);
    }
}
