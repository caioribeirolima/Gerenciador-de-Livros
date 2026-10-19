using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _dbContext;
        public UsuarioRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Usuarios.AnyAsync(u => u.Id == id, cancellationToken);
    }
}
