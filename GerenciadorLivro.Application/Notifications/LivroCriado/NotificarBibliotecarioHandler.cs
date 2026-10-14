using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.LivroCriado
{
    public class NotificarBibliotecarioHandler : INotificationHandler<LivroCriadoNotification>
    {
        private readonly ILogger<NotificarBibliotecarioHandler> _logger;

        public NotificarBibliotecarioHandler(ILogger<NotificarBibliotecarioHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(LivroCriadoNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notificando bibliotecário sobre o livro {LivroId} - {Titulo}", notification.LivroId, notification.Titulo);
            return Task.CompletedTask;
        }
    }
}
