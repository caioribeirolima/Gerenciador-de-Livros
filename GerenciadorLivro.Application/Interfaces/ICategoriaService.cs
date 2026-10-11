using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Services
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> ListarTodosAsync();
        Task<Categoria> CriarAsync(CriarCategoriaRequest request);
    }
}
