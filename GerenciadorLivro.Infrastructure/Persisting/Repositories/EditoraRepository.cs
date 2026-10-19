using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class EditoraRepository : IEditoraRepository
    {
        private readonly AppDbContext _dbContext;
        public EditoraRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AdicionarAsync(Editora editora, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Editora?> BuscarPorNomeAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VerificarSeExiste(string nome, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Editoras
                .Where(e => e.Nome.ToUpper() == nome.ToUpper())
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
