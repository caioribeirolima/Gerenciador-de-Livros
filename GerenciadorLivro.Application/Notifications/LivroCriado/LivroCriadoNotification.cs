using MediatR;

namespace GerenciadorLivro.Application.Notifications.LivroCriado
{
    public record LivroCriadoNotification(int LivroId, string Titulo) : INotification;

    public class EnviarBoasVindasHandler : INotificationHandler<LivroCriadoNotification>
    {
        public Task Handle(LivroCriadoNotification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
