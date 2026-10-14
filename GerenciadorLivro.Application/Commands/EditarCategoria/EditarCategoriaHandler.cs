using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.EditarCategoria
{
    public class EditarCategoriaHandler : IRequestHandler<EditarCategoriaCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        public EditarCategoriaHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(EditarCategoriaCommand command, CancellationToken cancellationToken)
        {
            var nomeLimpo = command.Nome.Trim();

            if (string.IsNullOrEmpty(nomeLimpo))
            {
                return Result.Falha("Nome da categoria inválido");
            }

            var categoria = await _dbContext.Categorias
                .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

            if (categoria == null)
            {
                return Result.Falha("Categoria inexistente");
            }

            var duplicada = await _dbContext.Categorias
                .AnyAsync(c => c.Id != command.Id && c.Nome.ToUpper() == nomeLimpo.ToUpper(), cancellationToken);

            if (duplicada)
            {
                return Result.Falha($"Categoria com nome {nomeLimpo} já existe");
            }

            categoria.Nome = nomeLimpo;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
