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
            _dbContext.Livros.Add(livro);
        }

        public void AdicionarAutorAoLivro(Livro livro, Autor autor)
        {
            livro.Autores.Add(autor);
        }

        public async Task<Livro?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Livros.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        public async Task<Livro?> BuscarPorIdIncluindoAutoresAsync(int id, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .Where(l => l.Id == id)
                .Include(l => l.Autores)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            return livro;
        }

        public async Task<Livro?> BuscarPorIdIncluindoExcluidosAsync(int id, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            return livro;
        }

        public void Excluir(Livro livro)
        {
            livro.Excluido = true;
        }

        public async Task<bool> ExisteLivroNaCategoriaAsync(int categoriaId, CancellationToken cancellationToken)
        {
            var possui = await _dbContext.Livros
                .IgnoreQueryFilters()
                .AnyAsync(l => l.CategoriaId == categoriaId, cancellationToken);

            return possui;
        }

        public async Task<bool> ExisteLivroNaEditoraAsync(int id, CancellationToken cancellationToken)
        {
            var possui = await _dbContext.Livros
                .IgnoreQueryFilters()
                .AnyAsync(l => l.EditoraId == id, cancellationToken);

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
