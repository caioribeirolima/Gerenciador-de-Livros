using MediatR;

namespace GerenciadorLivro.Application.Notifications.LivroDevolvido
{
    public record LivroDevolvidoNotification(int Id, DateTime DataPrevista, DateTime DataDevolucaoReal) : INotification;
}