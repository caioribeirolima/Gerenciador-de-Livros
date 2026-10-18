namespace GerenciadorLivro.Core.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}