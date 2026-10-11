using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ListarLivrosPorAno
{
    public class ListarLivrosPorAnoQuery : IRequest<Result<List<LivroResponse>>>
    {
        public int Ano { get; set; }
    }
}
