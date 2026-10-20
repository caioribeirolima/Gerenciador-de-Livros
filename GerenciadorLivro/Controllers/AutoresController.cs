using GerenciadorLivro.API.Extensions;
using GerenciadorLivro.Application.Commands.CriarAutor;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ListarAutores;
using GerenciadorLivro.Application.Queries.ObterAutorPorId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AutoresController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}", Name = "ObterAutorPorId")]
        public async Task<ActionResult<AutorDetalhesResponse>> ObterPorIdAsync(int id)
        {
            var result = await _mediator.Send(new ObterAutorPorIdQuery { Id = id });

            if (!result.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = $"Autor de id {id} não encontrado",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound,
                });
            }

            return Ok(result.Valor);
        }





        [HttpGet]
        public async Task<ActionResult<List<AutorResponse>>> ListarTodosAsync(
            [FromQuery] string? nome,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            var result = await _mediator.Send(new ListarAutoresQuery
            {
                Nome = nome,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
            });

            return Ok(result);
        }



        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarAutorRequest request)
        {
            var result = await _mediator.Send(new CriarAutorCommand
            {
                Nome = request.Nome,
            });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na criação do autor", result.ErrosValidacao);

            return CreatedAtRoute("ObterAutorPorId", new { id = result.Valor }, new { id = result.Valor });
        }



    }
}
