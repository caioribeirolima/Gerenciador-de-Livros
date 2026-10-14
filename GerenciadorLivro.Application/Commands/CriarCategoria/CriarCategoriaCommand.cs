using GerenciadorLivro.Application.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaCommand : IRequest<Result<int>>
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
    }
}
