using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarEditora
{
    public class CriarEditoraHandler : IRequestHandler<CriarEditoraCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;
        public CriarEditoraHandler(IAppDbContext dbContext) => _dbContext = dbContext;


        public async Task<Result<int>> Handle(CriarEditoraCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Nome))
            {
                return Result<int>.Falha("Nome da editora inválido");
            }

            var nome = command.Nome.Trim();

            if (nome.Length > 100)
            {
                return Result<int>.Falha("Nome não pode ter mais que 100 caracteres");
            }

            var exists = await _dbContext.Editoras
                .Where(e => e.Nome.ToUpper() == nome.ToUpper())
                .AnyAsync(cancellationToken);

            if (exists)
            {
                return Result<int>.Falha($"Editora com nome {nome} já existe");
            }

            var editora = new Editora
            {
                Nome = nome,
            };

            _dbContext.Editoras.Add(editora);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(editora.Id);
        }
    }
}
