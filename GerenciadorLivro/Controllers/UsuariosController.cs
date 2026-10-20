using GerenciadorLivro.API.Extensions;
using GerenciadorLivro.Application.Commands.CriarUsuario;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ObterUsuarioPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}", Name = "ObterUsuarioPorId")]
        public async Task<ActionResult<UsuarioResponse>> ObterPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ObterUsuarioPorIdQuery { Id = id }, cancellationToken);

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Usuário não encontrado", result.ErrosValidacao);

            return Ok(result.Valor);
        }

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CriarUsuarioCommand
            {
                Nome = request.Nome,
                Email = request.Email,
                Cpf = request.Cpf,
            }, cancellationToken);

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na criação do usuário", result.ErrosValidacao);

            return CreatedAtRoute("ObterUsuarioPorId", new { id = result.Valor }, new { id = result.Valor });
        }
    }
}
