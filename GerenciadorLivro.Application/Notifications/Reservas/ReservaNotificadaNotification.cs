using MediatR;

namespace GerenciadorLivro.Application.Notifications.Reservas
{
    public record ReservaNotificadaNotification(int ReservaId, int LivroId, int UsuarioId) : INotification;
}
