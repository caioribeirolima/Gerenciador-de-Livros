using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarReservasDoUsuario
{
    public class ListarReservasDoUsuarioQuery : IRequest<List<ReservaResponse>>
    {
        public int UsuarioId { get; set; }
    }
}
