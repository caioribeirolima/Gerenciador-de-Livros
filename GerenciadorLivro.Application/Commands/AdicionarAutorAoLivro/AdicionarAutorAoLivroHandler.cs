using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro
{
    public class AdicionarAutorAoLivroHandler : IRequestHandler<AdicionarAutorAoLivroCommand, Result>
    {
        private readonly IAppDbContext _dbContext;

        public AdicionarAutorAoLivroHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(AdicionarAutorAoLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .Include(l => l.Autores)
                .FirstOrDefaultAsync(l => l.Id == request.LivroId, cancellationToken);

            if (livro is null)
                return Result.Falha($"Livro de id {request.LivroId} não encontrado");

            var autor = await _dbContext.Autores
                .FirstOrDefaultAsync(a => a.Id == request.AutorId, cancellationToken);

            if (autor is null)
                return Result.Falha($"Autor de id {request.AutorId} não encontrado");

            if (livro.Autores.Any(a => a.Id == autor.Id))
                return Result.Falha($"O autor {request.AutorId} já está associado ao livro {request.LivroId}");

            livro.Autores.Add(autor);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
