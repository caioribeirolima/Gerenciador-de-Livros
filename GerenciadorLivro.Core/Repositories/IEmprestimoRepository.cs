using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IEmprestimoRepository
    {
        void Adicionar(Emprestimo emprestimo);
        Task<Emprestimo?> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> VerificarUsuarioComLivro(int livroId, int usuarioId, CancellationToken cancellationToken);
    }
}