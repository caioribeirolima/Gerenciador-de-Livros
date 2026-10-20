using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterLivroDetalhes
{
    public class ObterLivroDetalhesHandler : IRequestHandler<ObterLivroDetalhesQuery, Result<LivroDetalhesResponse>>
    {
        private readonly IAppDbContext _dbContext;

        public ObterLivroDetalhesHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<LivroDetalhesResponse>> Handle(ObterLivroDetalhesQuery request, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .AsNoTracking()
                .Where(l => l.Id == request.Id)
                .Select(l => new LivroDetalhesResponse
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    AnoPublicacao = l.AnoPublicacao,
                    Disponivel = l.Disponivel,
                    CategoriaNome = l.Categoria!.Nome,
                    EditoraNome = l.Editora!.Nome
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (livro is null)
                return Result<LivroDetalhesResponse>.NaoEncontrado($"Livro de id {request.Id} não encontrado");

            return Result<LivroDetalhesResponse>.Ok(livro);
        }
    }
}
