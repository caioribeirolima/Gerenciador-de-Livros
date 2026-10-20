using GerenciadorLivro.API.Extensions;
using GerenciadorLivro.Application.Commands.DevolverEmprestimo;
using GerenciadorLivro.Application.Commands.RegistrarEmprestimo;
using GerenciadorLivro.Application.Commands.RenovarEmprestimo;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.Queries.ListarEmprestimosAtivos;
using GerenciadorLivro.Application.Queries.ListarEmprestimosAtrasados;
using GerenciadorLivro.Application.Queries.ObterEmprestimoPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmprestimosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmprestimosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarEmprestimoRequest request)
        {
            var result = await _mediator.Send(new RegistrarEmprestimoCommand
            {
                LivroId = request.LivroId,
                UsuarioId = request.UsuarioId,
            });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na criação do empréstimo", result.ErrosValidacao);

            return CreatedAtRoute("BuscarEmprestimoPorId", new { id = result.Valor }, new { id = result.Valor });
        }

        [HttpGet("{id}", Name = "BuscarEmprestimoPorId")]
        public async Task<ActionResult> BuscarPorIdAsync(int id)
        {
            var result = await _mediator.Send(new ObterEmprestimoPorIdQuery { Id = id });

            if (!result.Sucesso)
                return NotFound(new ProblemDetails
                {
                    Title = "Empréstimo não encontrado",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound,
                });

            return Ok(result.Valor);
        }

        [HttpGet("ativos")]
        public async Task<ActionResult> ListarEmprestimosAtivosAsync()
        {
            var result = await _mediator.Send(new ListarEmprestimosAtivosQuery());
            return Ok(result);
        }

        [HttpGet("atrasados")]
        public async Task<ActionResult> ListarEmprestimosAtrasadosAsync()
        {
            var result = await _mediator.Send(new ListarEmprestimosAtrasadosQuery());
            return Ok(result);
        }

        [HttpPut("{id}/devolver")]
        public async Task<ActionResult> DevolverLivroAsync(int id)
        {
            var result = await _mediator.Send(new DevolverEmprestimoCommand
            {
                EmprestimoId = id,
            });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro ao devolver empréstimo");

            return NoContent();
        }


        [HttpPut("{id}/renovar")]
        public async Task<ActionResult> RenovarEmprestimoAsync(int id)
        {
            var result = await _mediator.Send(new RenovarEmprestimoCommand
            {
                EmprestimoId = id,
            });
            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao renovar empréstimo",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest,
                });
            }
            return NoContent();
        }
    }
}