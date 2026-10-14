using GerenciadorLivro.Application.Commands.CancelarReserva;
using GerenciadorLivro.Application.Commands.ReservarLivro;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ListarFilaDoLivro;
using GerenciadorLivro.Application.Queries.ListarReservasDoUsuario;
using GerenciadorLivro.Application.Queries.ObterReservaPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarReservaRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ReservarLivroCommand
            {
                LivroId = request.LivroId,
                UsuarioId = request.UsuarioId,
            }, cancellationToken);

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao reservar livro",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            return CreatedAtRoute("ObterReservaPorId", new { id = result.Valor }, new { id = result.Valor });
        }

        [HttpGet("{id:int}", Name = "ObterReservaPorId")]
        public async Task<ActionResult<ReservaResponse>> ObterPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ObterReservaPorIdQuery { Id = id }, cancellationToken);

            if (!result.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Reserva não encontrada",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound,
                });
            }

            return Ok(result.Valor);
        }

        [HttpGet("livro/{livroId:int}")]
        public async Task<ActionResult<List<ReservaResponse>>> ListarFilaDoLivroAsync(int livroId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarFilaDoLivroQuery { LivroId = livroId }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<ActionResult<List<ReservaResponse>>> ListarDoUsuarioAsync(int usuarioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarReservasDoUsuarioQuery { UsuarioId = usuarioId }, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}/cancelar")]
        public async Task<ActionResult> CancelarAsync(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelarReservaCommand { ReservaId = id }, cancellationToken);

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao cancelar reserva",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            return NoContent();
        }
    }
}
