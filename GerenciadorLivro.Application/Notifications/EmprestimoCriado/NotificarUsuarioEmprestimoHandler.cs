using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.EmprestimoCriado
{
    public class NotificarUsuarioEmprestimoHandler : INotificationHandler<EmprestimoRegistradoNotification>
    {
        private readonly ILogger<NotificarUsuarioEmprestimoHandler> _logger;

        public NotificarUsuarioEmprestimoHandler(ILogger<NotificarUsuarioEmprestimoHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmprestimoRegistradoNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Avisando {Usuario} que a devolução é em {Data}", notification.Usuario, notification.DataDevolucao.ToShortDateString());
            return Task.CompletedTask;
        }
    }
}