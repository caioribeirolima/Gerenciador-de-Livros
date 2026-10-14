using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroDevolvido;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GerenciadorLivro.Application.Notifications.Reservas
{
    // Quando o livro volta para a estante (devolução, ou a reserva Notificada foi cancelada),
    // a primeira reserva Ativa por DataReserva vira Notificada.
    public class AtenderProximaReservaHandler :
        INotificationHandler<LivroDevolvidoNotification>,
        INotificationHandler<ReservaCanceladaNotification>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly ILogger<AtenderProximaReservaHandler> _logger;

        public AtenderProximaReservaHandler(IAppDbContext dbContext, IPublisher publisher, ILogger<AtenderProximaReservaHandler> logger)
        {
            _dbContext = dbContext;
            _publisher = publisher;
            _logger = logger;
        }

        public Task Handle(LivroDevolvidoNotification notification, CancellationToken cancellationToken)
            => AtenderProximaAsync(notification.LivroId, cancellationToken);

        public Task Handle(ReservaCanceladaNotification notification, CancellationToken cancellationToken)
        {
            // Cancelar uma Ativa não libera o livro; cancelar a Notificada libera.
            if (notification.StatusAnterior != StatusReserva.Notificada)
            {
                return Task.CompletedTask;
            }

            return AtenderProximaAsync(notification.LivroId, cancellationToken);
        }

        private async Task AtenderProximaAsync(int livroId, CancellationToken cancellationToken)
        {
            var livroDisponivel = await _dbContext.Livros
                .AnyAsync(l => l.Id == livroId && l.Disponivel, cancellationToken);

            if (!livroDisponivel)
            {
                return;
            }

            var jaExisteNotificada = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == livroId && r.Status == StatusReserva.Notificada, cancellationToken);

            if (jaExisteNotificada)
            {
                return;
            }

            var proxima = await _dbContext.Reservas
                .Where(r => r.LivroId == livroId && r.Status == StatusReserva.Ativa)
                .OrderBy(r => r.DataReserva)
                .ThenBy(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (proxima == null)
            {
                return;
            }

            proxima.Status = StatusReserva.Notificada;
            proxima.DataNotificacao = DateTime.Now;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // O índice único filtrado barra duas Notificadas no mesmo livro. Mas só engolimos o erro
                // se outra requisição realmente notificou alguém; qualquer outra falha de banco sobe.
                var outraNotificada = await _dbContext.Reservas
                    .AsNoTracking()
                    .AnyAsync(r => r.LivroId == livroId && r.Status == StatusReserva.Notificada && r.Id != proxima.Id, cancellationToken);

                if (!outraNotificada)
                {
                    throw;
                }

                _logger.LogWarning(ex, "Outra reserva já foi notificada para o livro {LivroId}; a reserva {ReservaId} segue na fila", livroId, proxima.Id);
                return;
            }

            await _publisher.Publish(new ReservaNotificadaNotification(proxima.Id, proxima.LivroId, proxima.UsuarioId), cancellationToken);
        }
    }
}
