using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Core.Repositories
{
    public interface IEmprestimoRepository
    {
        void Adicionar(Emprestimo emprestimo);
    }
}