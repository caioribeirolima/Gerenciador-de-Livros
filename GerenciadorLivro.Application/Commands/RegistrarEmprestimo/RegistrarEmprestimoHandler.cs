using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.RegistrarEmprestimo
{
    public class RegistrarEmprestimoHandler : IRequestHandler<RegistrarEmprestimoCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;
        private readonly EmprestimoSettings _emprestimoSettings;
        public RegistrarEmprestimoHandler(IAppDbContext dbContext, EmprestimoSettings emprestimoSettings)
        {
            _dbContext = dbContext;
            _emprestimoSettings = emprestimoSettings;
        }

        public async Task<Result<int>> Handle(RegistrarEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .Where(l => l.Id == command.LivroId)
                .FirstOrDefaultAsync(cancellationToken);

            if (livro == null || livro.Disponivel == false)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} não está disponivel");
            }

            var emprestimo = new Emprestimo
            {
                LivroId = command.LivroId,
                UsuarioId = command.UsuarioId,
                DataEmprestimo = DateTime.Now,
                DataDevolucaoPrevista = DateTime.Now.AddDays(_emprestimoSettings.DiasParaDevolucao)
            };

            livro.Disponivel = false;
            _dbContext.Livros.Update(livro);

            await _dbContext.Emprestimos.AddAsync(emprestimo);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(emprestimo.Id);
        }
    }
}
