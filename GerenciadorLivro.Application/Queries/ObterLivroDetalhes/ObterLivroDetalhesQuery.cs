using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterLivroDetalhes
{
    public class ObterLivroDetalhesQuery : IRequest<Result<LivroDetalhesResponse>>
    {
        public int Id { get; set; }
    }
}
