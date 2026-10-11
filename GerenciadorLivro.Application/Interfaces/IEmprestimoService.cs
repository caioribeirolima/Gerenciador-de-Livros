using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Application.Interfaces
{
    public interface IEmprestimoService
    {
        Task<Result<Emprestimo>> CriarEmprestimoAsync(CriarEmprestimoRequest model);
        Task<Result> DevolverLivroAsync(int emprestimoId);
        Task<Result<EmprestimoResponse>> BuscarPorIdAsync(int id);
        Task<List<EmprestimoResponse>> ListarEmprestimosAtivosAsync();
        Task<List<EmprestimoResponse>> ListarEmprestimosAtrasadosAsync();
    }
}
