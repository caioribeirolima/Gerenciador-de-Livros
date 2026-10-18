using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _dbContext;
        public LivroRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Livros.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}