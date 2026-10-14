using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ExcluirEditora
{
    public class ExcluirEditoraHandler : IRequestHandler<ExcluirEditoraCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        public ExcluirEditoraHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(ExcluirEditoraCommand command, CancellationToken cancellationToken)
        {
            var editora = await _dbContext.Editoras
                .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

            if (editora == null)
            {
                return Result.Falha("Editora não existe");
            }

            var possuiLivros = await _dbContext.Livros
                .IgnoreQueryFilters()
                .AnyAsync(l => l.EditoraId == command.Id, cancellationToken);

            if (possuiLivros)
            {
                return Result.Falha("Existem livros vinculados a esta editora (incluindo livros excluídos)");
            }

            _dbContext.Editoras.Remove(editora);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
