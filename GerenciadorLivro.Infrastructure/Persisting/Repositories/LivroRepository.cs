using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories.Livros;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _dbContext;
        public LivroRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => _dbContext.Livros.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}