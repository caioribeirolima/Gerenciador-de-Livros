using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Infrastructure.Persisting;
using GerenciadorLivro.Services;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _dbContext;

        public CategoriaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Categoria>> ListarTodosAsync()
            => await _dbContext.Categorias.AsNoTracking().ToListAsync();

        public async Task<Categoria> CriarAsync(CriarCategoriaRequest request)
        {
            var categoria = new Categoria { Nome = request.Nome };
            await _dbContext.Categorias.AddAsync(categoria);
            await _dbContext.SaveChangesAsync();
            return categoria;
        }
    }
}
