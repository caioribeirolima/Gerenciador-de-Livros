using MediatR;

namespace GerenciadorLivro.Application.Notifications.LivroDevolvido
{
    public record LivroDevolvidoNotification(int Id, int LivroId, DateTime DataPrevista, DateTime DataDevolucaoReal) : INotification;
}
