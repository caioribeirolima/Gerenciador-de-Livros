using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorHandler : IRequestHandler<CriarAutorCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;
        public CriarAutorHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<int>> Handle(CriarAutorCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Nome))
            {
                return Result<int>.Falha("Nome de autor vazio");
            }

            var nome = command.Nome.Trim();

            if (nome.Length > 150)
            {
                return Result<int>.Falha("Nome não pode ter mais que 150 caracteres");
            }

            var exists = await _dbContext.Autores
                .Where(a => a.Nome.ToUpper() == nome.ToUpper())
                .AnyAsync(cancellationToken);

            if (exists)
            {
                return Result<int>.Falha($"Autor com nome {nome} já registrado");
            }

            var autor = new Autor
            {
                Nome = nome,
            };

            _dbContext.Autores.Add(autor);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(autor.Id);
        }
    }
}
