using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ExcluirLivro
{
    public class ExcluirLivroHandler : IRequestHandler<ExcluirLivroCommand, Result>
    {
        private readonly IAppDbContext _dbContext;

        public ExcluirLivroHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(ExcluirLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .Where(l => l.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (livro is null)
                return Result.Falha($"Livro de id {request.Id} não encontrado");

            livro.Excluido = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
