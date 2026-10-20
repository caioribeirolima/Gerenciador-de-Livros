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

        public void Adicionar(Editora editora)
        {
            _dbContext.Editoras.Add(editora);
        }

        public async Task<Editora?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var editora = await _dbContext.Editoras
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return editora;
        }

        public void Excluir(Editora editora)
        {
            _dbContext.Editoras.Remove(editora);
        }

        public async Task<bool> ExisteComNomeAsync(string nome, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Editoras
                .Where(e => e.Nome.ToUpper() == nome.ToUpper())
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
