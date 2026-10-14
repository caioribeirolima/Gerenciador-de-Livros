using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterReservaPorId
{
    public class ObterReservaPorIdQuery : IRequest<Result<ReservaResponse>>
    {
        public int Id { get; set; }
    }
}
