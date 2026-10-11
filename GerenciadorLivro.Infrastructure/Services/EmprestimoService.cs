using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Interfaces;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Exceptions;
using GerenciadorLivro.Infrastructure.Persisting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.Infrastructure.Services
{
    public class EmprestimoService : IEmprestimoService
    {

        private readonly AppDbContext _dbContext;
        private readonly EmprestimoSettings _emprestimoSettings;

        public EmprestimoService(AppDbContext dbContext, IOptions<EmprestimoSettings> emprestimoSettings)
        {
            _dbContext = dbContext;
            _emprestimoSettings = emprestimoSettings.Value;
        }

        public async Task<Result<Emprestimo>> CriarEmprestimoAsync(CriarEmprestimoRequest model)
        {
            var livro = await _dbContext.Livros.FindAsync(model.LivroId);

            if(livro == null || livro.Disponivel == false)
            {
                return Result<Emprestimo>.Falha($"Livro de id {model.LivroId} não está disponivel");
            }

            var emprestimo = new Emprestimo
            {
                LivroId = model.LivroId,
                UsuarioId = model.UsuarioId,
                DataEmprestimo = DateTime.Now,
                DataDevolucaoPrevista = DateTime.Now.AddDays(_emprestimoSettings.DiasParaDevolucao)
            };

            livro.Disponivel = false;
            _dbContext.Livros.Update(livro);

            await _dbContext.Emprestimos.AddAsync(emprestimo);
            await _dbContext.SaveChangesAsync();

            return Result<Emprestimo>.Ok(emprestimo);

        }

        public async Task<Result> DevolverLivroAsync(int emprestimoId)
        {
            var resultado = await _dbContext.Emprestimos.FindAsync(emprestimoId);

            if(resultado == null)
            {
                return Result.Falha($"Empréstimo de id {emprestimoId} não encontrado");
            }

            resultado.DataDevolucaoReal = DateTime.Now;
            _dbContext.Emprestimos.Update(resultado);

            var livro = await _dbContext.Livros
                .FindAsync(resultado.LivroId);

            if(livro == null)
            {
                return Result.Falha($"Livro vinculado ao empréstimo de id {emprestimoId} não foi encontrado");
            }

            livro.Disponivel = true;
            _dbContext.Livros.Update(livro);

            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }

        private EmprestimoResponse ParaResponse(Emprestimo e) => new EmprestimoResponse
        {
            Id = e.Id,
            LivroTitulo = e.Livro?.Titulo ?? "—",
            UsuarioNome = e.Usuario?.Nome ?? "—",
            DataEmprestimo = e.DataEmprestimo,
            DataDevolucaoPrevista = e.DataDevolucaoPrevista,
            DataDevolucaoReal = e.DataDevolucaoReal
        };

        public async Task<List<EmprestimoResponse>> ListarEmprestimosAtivosAsync()
        {
            var livrosAtivos = await _dbContext.Emprestimos
                .Where(e => e.DataDevolucaoReal == null)
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .AsNoTracking()
                .ToListAsync();

            return livrosAtivos.Select(ParaResponse).ToList();
        }

        public async Task<List<EmprestimoResponse>> ListarEmprestimosAtrasadosAsync()
        {
            var emprestimosAtrasados = await _dbContext.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .Where(e => e.DataDevolucaoReal == null && e.DataDevolucaoPrevista < DateTime.Now.Date)
                .AsNoTracking()
                .ToListAsync();

            return emprestimosAtrasados.Select(ParaResponse).ToList();
        }

        public async Task<Result<EmprestimoResponse>> BuscarPorIdAsync(int id)
        {
            var resultado = await _dbContext.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (resultado == null)
            {
                return Result<EmprestimoResponse>.Falha($"Empréstimo de id {id} não encontrado");
            }

            return Result<EmprestimoResponse>.Ok(ParaResponse(resultado));
        }
    }
}
