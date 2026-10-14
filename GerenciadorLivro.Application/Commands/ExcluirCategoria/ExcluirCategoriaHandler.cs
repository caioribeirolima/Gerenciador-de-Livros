using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ExcluirCategoria
{
    public class ExcluirCategoriaHandler : IRequestHandler<ExcluirCategoriaCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        public ExcluirCategoriaHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(ExcluirCategoriaCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _dbContext.Categorias
                .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

            if (categoria == null)
            {
                return Result.Falha("Categoria não existe");
            }

            var possuiLivros = await _dbContext.Livros
                .IgnoreQueryFilters()
                .AnyAsync(l => l.CategoriaId == command.Id, cancellationToken);

            if (possuiLivros)
            {
                return Result.Falha("Existem livros vinculados a esta categoria (incluindo livros excluídos)");
            }

            _dbContext.Categorias.Remove(categoria);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
