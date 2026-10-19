using Azure.Core;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly AppDbContext _dbContext;
        public AutorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CriarAutor(Autor autor)
        {
            _dbContext.Autores.AddAsync(autor);
        }

        public async Task<Autor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var autor = await _dbContext.Autores
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            return autor;
        }

        public async Task<List<Autor>> ListarPorIdAsync(List<int> autoresId, CancellationToken cancellationToken)
        {
            var autores = await _dbContext.Autores
                .Where(a => autoresId.Contains(a.Id))
                .ToListAsync(cancellationToken);

            return autores;
        }

        public async Task<bool> BuscarPorNomeAsync(string nome, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Autores
                .Where(a => a.Nome.ToUpper() == nome.ToUpper())
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
