using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaHandler : IRequestHandler<CriarCategoriaCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;

        public CriarCategoriaHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<int>> Handle(CriarCategoriaCommand command, CancellationToken cancellationToken)
        {

            var stringLimpa = command.Nome.Trim();

            if (string.IsNullOrEmpty(stringLimpa))
            {
                return Result<int>.Falha("Nome da categoria inválido");
            }

            var exists = await _dbContext.Categorias
                .AnyAsync(c => c.Nome.ToUpper() == stringLimpa.ToUpper(), cancellationToken);

            if (exists)
            {
                return Result<int>.Falha("Uma categoria já está cadastrada com este nome");
            }

            var categoria = new Categoria
            {
                Nome = stringLimpa,
            };

            _dbContext.Categorias.Add(categoria);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Ok(categoria.Id);


        }
    }
}
