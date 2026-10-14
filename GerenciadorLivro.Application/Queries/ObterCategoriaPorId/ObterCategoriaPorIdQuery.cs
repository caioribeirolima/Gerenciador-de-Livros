using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterCategoriaPorId
{
    public class ObterCategoriaPorIdQuery : IRequest<Result<CategoriaResponse>>
    {
        public int Id { get; set; }
    }
}
