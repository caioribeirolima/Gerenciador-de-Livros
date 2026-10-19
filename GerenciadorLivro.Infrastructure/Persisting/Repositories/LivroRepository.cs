using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _dbContext;
        public LivroRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public void Adicionar(Livro livro)
        {
            _dbContext.Livros.AddAsync(livro);
        }

        public async Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Livros.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);


        public async Task<Livro?> BuscarPorIdIncluindoExcluidosAsync(int id, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            return livro;
        }

        public async Task<bool> ExisteLivroNaCategoriaAsync(int categoriaId, CancellationToken cancellationToken)
        {
            var possui = await _dbContext.Livros
                .IgnoreQueryFilters()
                .AnyAsync(l => l.CategoriaId == categoriaId, cancellationToken);

            return possui;
        }

        public async Task<bool> VerificarLivroDisponivel(int id, CancellationToken cancellationToken)
        {
            var livroDisponivel = await _dbContext.Livros
                .AnyAsync(l => l.Id == id && l.Disponivel, cancellationToken);

            return livroDisponivel;
        }
    }
}
