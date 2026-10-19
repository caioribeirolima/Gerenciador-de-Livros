using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _dbContext;
        public CategoriaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Categoria?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var categoria = await _dbContext.Categorias
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return categoria;
        }

        public async void CriarAsync(Categoria categoria, CancellationToken cancellationToken)
        {
            await _dbContext.Categorias.AddAsync(categoria, cancellationToken);
        }

        public void EditarAsync(Categoria categoria, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void ExcluirAsync(Categoria categoria)
        {
            _dbContext.Categorias.Remove(categoria);
        }

        public async Task<bool> VerificarDuplicidadePorNome(string nome, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Categorias
                .AnyAsync(c => c.Nome.ToUpper() == nome.ToUpper(), cancellationToken);

            return exists;
        }
    }
}
