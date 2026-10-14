using GerenciadorLivro.Application.Commands.CriarAutor;
using GerenciadorLivro.Application.DTOs.InputModels;
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

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarAutorRequest request)
        {
            var result = await _mediator.Send(new CriarAutorCommand());

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao criar autor",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            return CreatedAtRoute("BuscarPorId", new { id = result.Valor }, new { id = result.Valor });
        }
    }
}
