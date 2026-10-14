using MediatR;

namespace GerenciadorLivro.Application.Notifications.EmprestimoCriado
{
    public record EmprestimoRegistradoNotification(int Id, string Usuario, DateTime DataDevolucao) : INotification;
}