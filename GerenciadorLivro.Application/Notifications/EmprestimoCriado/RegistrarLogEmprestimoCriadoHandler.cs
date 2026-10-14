using MediatR;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.EmprestimoCriado
{
    public class RegistrarLogEmprestimoCriadoHandler : INotificationHandler<EmprestimoRegistradoNotification>
    {
        private readonly ILogger<RegistrarLogEmprestimoCriadoHandler> _logger;

        public RegistrarLogEmprestimoCriadoHandler(ILogger<RegistrarLogEmprestimoCriadoHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmprestimoRegistradoNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Auditoria: Empréstimo registrado com ID {Id} para o usuário {Usuario}.", notification.Id, notification.Usuario);
            return Task.CompletedTask;
        }
    }
}