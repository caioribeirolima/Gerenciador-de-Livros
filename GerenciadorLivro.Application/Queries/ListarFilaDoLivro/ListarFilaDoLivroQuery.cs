using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarFilaDoLivro
{
    public class ListarFilaDoLivroQuery : IRequest<List<ReservaResponse>>
    {
        public int LivroId { get; set; }
    }
}
