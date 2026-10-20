using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterUsuarioPorId
{
    public class ObterUsuarioPorIdQuery : IRequest<Result<UsuarioResponse>>
    {
        public int Id { get; set; }
    }
}
