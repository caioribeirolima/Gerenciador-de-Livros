using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Notifications.Reservas;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CancelarReserva
{
    public class CancelarReservaHandler : IRequestHandler<CancelarReservaCommand, Result>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public CancelarReservaHandler(IReservaRepository reservaRepository, IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _reservaRepository = reservaRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<Result> Handle(CancelarReservaCommand command, CancellationToken cancellationToken)
        {
            var reserva = await _reservaRepository.BuscarPorIdAsync(command.ReservaId, cancellationToken);

            if (reserva == null)
                return Result.NaoEncontrado($"Reserva de id {command.ReservaId} não encontrada");

            if (reserva.Status != StatusReserva.Ativa && reserva.Status != StatusReserva.Notificada)
                return Result.Conflito($"Reserva de id {command.ReservaId} não pode ser cancelada: status atual é {reserva.Status}");

            var statusAnterior = reserva.Status;
            reserva.Status = StatusReserva.Cancelada;
            reserva.DataEncerramento = DateTime.Now;

            await _unitOfWork.CommitAsync(cancellationToken);

            await _publisher.Publish(new ReservaCanceladaNotification(reserva.Id, reserva.LivroId, statusAnterior), cancellationToken);

            return Result.Ok();
        }
    }
}
