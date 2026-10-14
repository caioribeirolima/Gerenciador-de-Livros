using MediatR;

namespace GerenciadorLivro.Application.Notifications.LivroDevolvido
{
    public record LivroDevolvidoNotification(int EmprestimoId, int LivroId, DateTime DataPrevista, DateTime DataDevolucaoReal) : INotification;
}
