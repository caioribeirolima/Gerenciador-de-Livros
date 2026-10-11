using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroHandler : IRequestHandler<CriarLivroCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;

        public CriarLivroHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<int>> Handle(CriarLivroCommand command, CancellationToken cancellationToken)
        {
            if (command.AnoPublicacao > DateTime.Now.Year)
                return Result<int>.Falha($"Livro com ano de publicação {command.AnoPublicacao} inválido");

            var autores = await _dbContext.Autores
                .Where(a => command.AutorIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            var livro = new Livro
            {
                Titulo = command.Titulo,
                AnoPublicacao = command.AnoPublicacao,
                CategoriaId = command.CategoriaId,
                EditoraId = command.EditoraId,
                Autores = autores
            };

            _dbContext.Livros.Add(livro);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(livro.Id);
        }
    }
}
