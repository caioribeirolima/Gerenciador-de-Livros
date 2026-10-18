using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroDevolvido;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.DevolverEmprestimo
{
    public class DevolverEmprestimoHandler : IRequestHandler<DevolverEmprestimoCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly IEmprestimoRepository _emprestimoRepository;

        public DevolverEmprestimoHandler(IAppDbContext dbContext, IPublisher publisher, IEmprestimoRepository emprestimoRepository)
        {
            _dbContext = dbContext;
            _publisher = publisher;
            _emprestimoRepository = emprestimoRepository;
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

            // IgnoreQueryFilters: o livro pode ter sido excluído (soft delete) com o empréstimo ainda aberto.
            // Sem isso o filtro o esconderia e a devolução ficaria impossível.
            var livro = await _dbContext.Livros
                .IgnoreQueryFilters()
                .Where(l => l.Id == resultado.LivroId)
                .FirstOrDefaultAsync(cancellationToken);

            if (livro == null)
            {
                return Result.Falha($"Livro vinculado ao empréstimo de id {resultado.LivroId} não foi encontrado");
            }

            // As duas entidades já estão rastreadas: o SaveChanges grava as mudanças, sem Update.
            resultado.DataDevolucaoReal = DateTime.Now;
            livro.Disponivel = true;

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
