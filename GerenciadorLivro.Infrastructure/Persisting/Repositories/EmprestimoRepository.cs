using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly AppDbContext _dbContext;
        public EmprestimoRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public void Adicionar(Emprestimo emprestimo) => _dbContext.Emprestimos.Add(emprestimo);
    }
}