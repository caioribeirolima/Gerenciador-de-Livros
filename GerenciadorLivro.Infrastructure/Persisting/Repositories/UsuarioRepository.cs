using GerenciadorLivro.Core.Entities;
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

        public async Task<Usuario?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public async Task<bool> ExisteComCpfAsync(string cpf, CancellationToken cancellationToken)
            => await _dbContext.Usuarios.AnyAsync(u => u.Cpf == cpf, cancellationToken);

        public void Adicionar(Usuario usuario) => _dbContext.Usuarios.Add(usuario);

        public void Remover(Usuario usuario) => _dbContext.Usuarios.Remove(usuario);
    }
}
