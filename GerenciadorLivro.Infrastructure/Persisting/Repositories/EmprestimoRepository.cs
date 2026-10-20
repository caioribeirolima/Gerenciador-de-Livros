using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting.Repositories
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly AppDbContext _dbContext;
        public EmprestimoRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public void Adicionar(Emprestimo emprestimo) => _dbContext.Emprestimos.Add(emprestimo);

        public async Task<Emprestimo?> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
            => await _dbContext.Emprestimos
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public async Task<bool> ExisteEmprestimoAbertoDoUsuarioAsync(int livroId, int usuarioId, CancellationToken cancellationToken)
        {
            var usuarioEstaComLivro = await _dbContext.Emprestimos
                .AnyAsync(e => e.LivroId == livroId
                && e.UsuarioId == usuarioId
                && e.DataDevolucaoReal == null, cancellationToken);

            return usuarioEstaComLivro;
        }

        // Ativo = ainda não devolvido, inclusive de livro excluído do acervo (o empréstimo continua em aberto).
        public async Task<int> ContarAtivosDoUsuarioAsync(int usuarioId, CancellationToken cancellationToken)
            => await _dbContext.Emprestimos
                .CountAsync(e => e.UsuarioId == usuarioId && e.DataDevolucaoReal == null, cancellationToken);
    }
}