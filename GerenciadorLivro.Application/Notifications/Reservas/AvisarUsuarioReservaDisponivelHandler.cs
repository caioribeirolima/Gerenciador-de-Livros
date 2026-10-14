using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.Reservas
{
    public class AvisarUsuarioReservaDisponivelHandler : INotificationHandler<ReservaNotificadaNotification>
    {
        private readonly ILogger<AvisarUsuarioReservaDisponivelHandler> _logger;

        public AvisarUsuarioReservaDisponivelHandler(ILogger<AvisarUsuarioReservaDisponivelHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ReservaNotificadaNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Avisando usuário {UsuarioId}: o livro {LivroId} da reserva {ReservaId} está disponível para retirada",
                notification.UsuarioId, notification.LivroId, notification.ReservaId);
            return Task.CompletedTask;
        }
    }
}
