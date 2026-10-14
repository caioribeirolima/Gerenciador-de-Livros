using GerenciadorLivro.Core.Entities;
using System.Text.Json.Serialization;

namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class ReservaResponse
    {
        public int Id { get; set; }
        public int LivroId { get; set; }
        public string LivroTitulo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public DateTime DataReserva { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusReserva Status { get; set; }

        public DateTime? DataNotificacao { get; set; }

        // 1 = é a vez dessa reserva. Nulo para reservas Atendidas ou Canceladas.
        public int? Posicao { get; set; }
    }
}
