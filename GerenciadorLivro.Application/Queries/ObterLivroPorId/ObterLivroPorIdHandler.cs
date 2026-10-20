using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Queries.ObterLivroPorId
{
    public class ObterLivroPorIdHandler : IRequestHandler<ObterLivroPorIdQuery, Result<LivroResponse>>
    {
        private readonly IAppDbContext _dbContext;

        public ObterLivroPorIdHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<LivroResponse>> Handle(ObterLivroPorIdQuery query, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .AsNoTracking()
                .Where(l => l.Id == query.Id)
                .Select(l => new LivroResponse
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    AnoPublicacao = l.AnoPublicacao,
                    Disponivel = l.Disponivel
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (livro is null)
                return Result<LivroResponse>.NaoEncontrado($"Livro de id {query.Id} não encontrado");

            return Result<LivroResponse>.Ok(livro);
        }
    }
}
