using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.LivroDevolvido
{
    public class VerificarAtrasoDevolucaoHandler : INotificationHandler<LivroDevolvidoNotification>
    {
        private readonly ILogger<VerificarAtrasoDevolucaoHandler> _logger;

        public VerificarAtrasoDevolucaoHandler(ILogger<VerificarAtrasoDevolucaoHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(LivroDevolvidoNotification notification, CancellationToken cancellationToken)
        {
            if (notification.DataDevolucaoReal > notification.DataPrevista)
            {
                var diasAtraso = (notification.DataDevolucaoReal - notification.DataPrevista).Days;
                _logger.LogWarning("Livro {Id} devolvido com atraso. Dias de atraso: {DiasAtraso}", notification.Id, diasAtraso);
            }

            return Task.CompletedTask;
        }
    }
}