using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Queries.ObterUsuarioPorId
{
    public class ObterUsuarioPorIdHandler : IRequestHandler<ObterUsuarioPorIdQuery, Result<UsuarioResponse>>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public ObterUsuarioPorIdHandler(IUsuarioRepository usuarioRepository) => _usuarioRepository = usuarioRepository;

        public async Task<Result<UsuarioResponse>> Handle(ObterUsuarioPorIdQuery query, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(query.Id, cancellationToken);

            if (usuario == null)
                return Result<UsuarioResponse>.NaoEncontrado($"Usuário de id {query.Id} não encontrado");

            return Result<UsuarioResponse>.Ok(new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                CpfMascarado = Cpf.Mascarar(usuario.Cpf)
            });
        }
    }
}
