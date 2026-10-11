using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterLivroPorId
{
    public class ObterLivroPorIdQuery : IRequest<Result<LivroResponse>>
    {
        public int Id { get; set; }
    }
}
