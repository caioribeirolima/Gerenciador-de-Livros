using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Application.Common.Mappings
{
    public static class ReservaMappings
    {
        // A posição não é gravada: é derivada de Status + DataReserva (+ Id para desempate)
        // no momento da leitura, dentro da própria query SQL.
        // Notificada é sempre a 1ª; uma Ativa vem depois da Notificada e das Ativas mais antigas.
        public static IQueryable<ReservaResponse> ProjectToResponse(
            this IQueryable<Reserva> query,
            IQueryable<Reserva> todasAsReservas)
        {
            return query.Select(r => new ReservaResponse
            {
                Id = r.Id,
                LivroId = r.LivroId,
                LivroTitulo = r.Livro != null ? r.Livro.Titulo : string.Empty,
                UsuarioId = r.UsuarioId,
                UsuarioNome = r.Usuario != null ? r.Usuario.Nome : string.Empty,
                DataReserva = r.DataReserva,
                Status = r.Status,
                DataNotificacao = r.DataNotificacao,
                Posicao = r.Status == StatusReserva.Notificada
                    ? 1
                    : r.Status == StatusReserva.Ativa
                        ? 1 + todasAsReservas.Count(o =>
                            o.LivroId == r.LivroId &&
                            (o.Status == StatusReserva.Notificada ||
                             (o.Status == StatusReserva.Ativa &&
                              (o.DataReserva < r.DataReserva ||
                               (o.DataReserva == r.DataReserva && o.Id < r.Id)))))
                        : (int?)null
            });
        }
    }
}
