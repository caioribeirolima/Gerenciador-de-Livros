using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterAutorPorId
{
    public class ObterAutorPorIdQuery : IRequest<Result<AutorResponse>>
    {
        public int Id { get; set; }
    }
}
