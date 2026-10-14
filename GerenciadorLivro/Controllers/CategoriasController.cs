using GerenciadorLivro.Application.Commands.CriarCategoria;
using GerenciadorLivro.Application.Commands.EditarCategoria;
using GerenciadorLivro.Application.Commands.ExcluirCategoria;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ListarCategorias;
using GerenciadorLivro.Application.Queries.ObterCategoriaPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaResponse>>> ListarTodosAsync(
            [FromQuery] string? nome,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            var categorias = await _mediator.Send(new ListarCategoriasQuery
            {
                Nome = nome,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            });

            return Ok(categorias);
        }

        [HttpGet("{id:int}", Name = "ObterCategoriaPorId")]
        public async Task<ActionResult<CategoriaResponse>> ObterPorIdAsync(int id)
        {
            var result = await _mediator.Send(new ObterCategoriaPorIdQuery { Id = id });

            if (!result.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Categoria não encontrada",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Valor);
        }

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarCategoriaRequest request)
        {
            var result = await _mediator.Send(new CriarCategoriaCommand { Nome = request.Nome });

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao criar categoria",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return CreatedAtRoute("ObterCategoriaPorId", new { id = result.Valor }, new { id = result.Valor });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditarAsync(int id, EditarCategoriaRequest request)
        {
            var result = await _mediator.Send(new EditarCategoriaCommand { Id = id, Nome = request.Nome });

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao editar categoria",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> ExcluirAsync(int id)
        {
            var result = await _mediator.Send(new ExcluirCategoriaCommand { Id = id });

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao excluir categoria",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return NoContent();
        }
    }
}
