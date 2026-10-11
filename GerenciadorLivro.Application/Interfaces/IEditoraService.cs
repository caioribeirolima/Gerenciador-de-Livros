using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Services
{
    public interface IEditoraService
    {
        Task<List<Editora>> ListarTodosAsync();
        Task<Editora> CriarAsync(CriarEditoraRequest request);
    }
}
