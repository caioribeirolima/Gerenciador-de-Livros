using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroDevolvido;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.DevolverEmprestimo
{
    public class DevolverEmprestimoHandler : IRequestHandler<DevolverEmprestimoCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IPublisher _publisher;

        public DevolverEmprestimoHandler(IAppDbContext dbContext, IPublisher publisher)
        {
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task<Result> Handle(DevolverEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var resultado = await _dbContext.Emprestimos
                .Where(e => e.Id == command.EmprestimoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (resultado == null)
            {
                return Result.Falha($"Empréstimo de id {command.EmprestimoId} não encontrado");
            }

            if (resultado.DataDevolucaoReal != null)
            {
                return Result.Falha($"Empréstimo de id {command.EmprestimoId} já foi devolvido");
            }

            resultado.DataDevolucaoReal = DateTime.Now;
            _dbContext.Emprestimos.Update(resultado);

            var livro = await _dbContext.Livros
                .Where(l => l.Id == resultado.LivroId)
                .FirstOrDefaultAsync(cancellationToken);

            if (livro == null)
            {
                return Result.Falha($"Livro vinculado ao empréstimo de id {resultado.LivroId} não foi encontrado");
            }

            livro.Disponivel = true;
            _dbContext.Livros.Update(livro);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new LivroDevolvidoNotification(
                resultado.Id,
                resultado.LivroId,
                resultado.DataDevolucaoPrevista,
                resultado.DataDevolucaoReal.Value), cancellationToken);

            return Result.Ok();
        }
    }
}
