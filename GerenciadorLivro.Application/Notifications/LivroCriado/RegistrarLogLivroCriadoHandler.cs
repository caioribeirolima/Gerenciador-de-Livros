using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.LivroCriado
{
    public class RegistrarLogLivroCriadoHandler : INotificationHandler<LivroCriadoNotification>
    {
        private readonly ILogger<RegistrarLogLivroCriadoHandler> _logger;

        public RegistrarLogLivroCriadoHandler(ILogger<RegistrarLogLivroCriadoHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(LivroCriadoNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Livro {LivroId} criado: {Titulo}", notification.LivroId, notification.Titulo);
            return Task.CompletedTask;
        }
    }
}