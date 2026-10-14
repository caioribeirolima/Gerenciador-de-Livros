using GerenciadorLivro.Core.Entities;
using MediatR;

namespace GerenciadorLivro.Application.Notifications.Reservas
{
    public record ReservaCanceladaNotification(int ReservaId, int LivroId, StatusReserva StatusAnterior) : INotification;
}
