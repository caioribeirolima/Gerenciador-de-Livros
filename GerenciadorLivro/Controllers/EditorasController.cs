using GerenciadorLivro.API.Extensions;
using GerenciadorLivro.Application.Commands.CriarEditora;
using GerenciadorLivro.Application.Commands.ExcluirEditora;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ListarEditoras;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EditorasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EditorasResponse>>> ListarTodosAsync(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            var result = await _mediator.Send(new ListarEditorasQuery
            {
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarEditoraRequest request)
        {
            var result = await _mediator.Send(new CriarEditoraCommand
            {
                Nome = request.Nome,
            });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na criação da editora", result.ErrosValidacao);

            return Created($"/api/editoras/{result.Valor}", new { id = result.Valor });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> ExcluirAsync(int id)
        {
            var result = await _mediator.Send(new ExcluirEditoraCommand { Id = id });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro ao excluir editora");

            return NoContent();
        }
    }
}
