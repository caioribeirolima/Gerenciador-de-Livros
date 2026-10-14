using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterAutorPorId
{
    public class ObterAutorPorIdQuery : IRequest<Result<AutorDetalhesResponse>>
    {
        public int Id { get; set; }
    }
}
