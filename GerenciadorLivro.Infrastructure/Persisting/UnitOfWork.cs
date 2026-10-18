using GerenciadorLivro.Core.Repositories;

namespace GerenciadorLivro.Infrastructure.Persisting
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public UnitOfWork(AppDbContext dbContext) => _dbContext = dbContext;

        public Task<int> CommitAsync(CancellationToken cancellationToken)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}