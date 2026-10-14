using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CancelarReserva
{
    public class CancelarReservaCommand : IRequest<Result>
    {
        public int ReservaId { get; set; }
    }
}
