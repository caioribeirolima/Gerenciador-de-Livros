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
            // Compara só a data: devolver no dia do prazo, a qualquer hora, não é atraso.
            if (notification.DataDevolucaoReal.Date > notification.DataPrevista.Date)
            {
                var diasAtraso = (notification.DataDevolucaoReal.Date - notification.DataPrevista.Date).Days;
                _logger.LogWarning("Empréstimo {EmprestimoId} (livro {LivroId}) devolvido com atraso. Dias de atraso: {DiasAtraso}",
                    notification.EmprestimoId, notification.LivroId, diasAtraso);
            }

            return Task.CompletedTask;
        }
    }
}