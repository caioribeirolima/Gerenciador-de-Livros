using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Infrastructure.Persisting;
using GerenciadorLivro.Services;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Services
{
    public class EditoraService : IEditoraService
    {
        private readonly AppDbContext _dbContext;

        public EditoraService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Editora>> ListarTodosAsync()
            => await _dbContext.Editoras.AsNoTracking().ToListAsync();

        public async Task<Editora> CriarAsync(CriarEditoraRequest request)
        {
            var editora = new Editora { Nome = request.Nome };
            await _dbContext.Editoras.AddAsync(editora);
            await _dbContext.SaveChangesAsync();
            return editora;
        }
    }
}
