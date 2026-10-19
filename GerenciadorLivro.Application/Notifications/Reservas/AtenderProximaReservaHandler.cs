using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroDevolvido;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.Reservas
{
    public class AtenderProximaReservaHandler :
        INotificationHandler<LivroDevolvidoNotification>,
        INotificationHandler<ReservaCanceladaNotification>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;
        private readonly ILogger<AtenderProximaReservaHandler> _logger;

        public AtenderProximaReservaHandler(
            ILivroRepository livroRepository,
            IReservaRepository reservaRepository,
            IUnitOfWork unitOfWork,
            IPublisher publisher, 
            ILogger<AtenderProximaReservaHandler> logger)
        {
            _livroRepository = livroRepository;
            _reservaRepository = reservaRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _logger = logger;
        }

        public Task Handle(LivroDevolvidoNotification notification, CancellationToken cancellationToken)
            => AtenderProximaAsync(notification.LivroId, cancellationToken);

        public Task Handle(ReservaCanceladaNotification notification, CancellationToken cancellationToken)
        {
            if (notification.StatusAnterior != StatusReserva.Notificada)
            {
                return Task.CompletedTask;
            }

            return AtenderProximaAsync(notification.LivroId, cancellationToken);
        }

        private async Task AtenderProximaAsync(int livroId, CancellationToken cancellationToken)
        {
            var livroDisponivel = await _livroRepository
                .VerificarLivroDisponivel(livroId, cancellationToken);

            if (!livroDisponivel)
            {
                return;
            }

            var jaExisteNotificada = await _reservaRepository
                .ExisteReservaLivroNotificada(livroId, cancellationToken);

            if (jaExisteNotificada)
            {
                return;
            }

            var proxima = await _reservaRepository
                .BuscarReservaAtivaOrdenadaPorNome(livroId, cancellationToken);


            if (proxima == null)
            {
                return;
            }

            proxima.Status = StatusReserva.Notificada;
            proxima.DataNotificacao = DateTime.Now;

            try
            {
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                var outraNotificada = await _reservaRepository
                    .ExisteReservaNotificadaDeOutroUsuarioAsync(livroId, proxima.UsuarioId, cancellationToken);
                
                if (!outraNotificada)
                {
                    throw;
                }

                _logger.LogWarning(
                    ex, 
                    "Outra reserva já foi notificada para o livro {LivroId}; a reserva {ReservaId} segue na fila", 
                    livroId, proxima.Id);

                return;
            }

            await _publisher.Publish(
                new ReservaNotificadaNotification(
                    proxima.Id, proxima.LivroId, proxima.UsuarioId), cancellationToken);
        }
    }
}
