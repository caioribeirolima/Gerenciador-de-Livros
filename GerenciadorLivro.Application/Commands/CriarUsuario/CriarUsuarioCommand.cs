using GerenciadorLivro.Application.Common;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarUsuario
{
    public class CriarUsuarioCommand : IRequest<Result<int>>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Chega como o cliente digitou (com ou sem máscara); o handler grava só os dígitos.
        public string Cpf { get; set; } = string.Empty;
    }
}
