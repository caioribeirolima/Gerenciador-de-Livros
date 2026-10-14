using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.Reservas;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CancelarReserva
{
    public class CancelarReservaHandler : IRequestHandler<CancelarReservaCommand, Result>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IPublisher _publisher;

        public CancelarReservaHandler(IAppDbContext dbContext, IPublisher publisher)
        {
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task<Result> Handle(CancelarReservaCommand command, CancellationToken cancellationToken)
        {
            var reserva = await _dbContext.Reservas
                .Where(r => r.Id == command.ReservaId)
                .FirstOrDefaultAsync(cancellationToken);

            if (reserva == null)
            {
                return Result.Falha($"Reserva de id {command.ReservaId} não encontrada");
            }

            if (reserva.Status != StatusReserva.Ativa && reserva.Status != StatusReserva.Notificada)
            {
                return Result.Falha($"Reserva de id {command.ReservaId} não pode ser cancelada: status atual é {reserva.Status}");
            }

            var statusAnterior = reserva.Status;
            reserva.Status = StatusReserva.Cancelada;
            reserva.DataEncerramento = DateTime.Now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new ReservaCanceladaNotification(reserva.Id, reserva.LivroId, statusAnterior), cancellationToken);

            return Result.Ok();
        }
    }
}
