using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.Interfaces;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmprestimosController : ControllerBase
    {
        private readonly EmprestimoSettings _emprestimoSettings;
        private readonly IEmprestimoService _emprestimoService;

        public EmprestimosController(
            IEmprestimoService emprestimoService,
            IOptions<EmprestimoSettings> emprestimoSettings)
        {
            _emprestimoService = emprestimoService;
            _emprestimoSettings = emprestimoSettings.Value;
        }


        [HttpPost]
        public async Task<ActionResult> CriarEmprestimoAsync(CriarEmprestimoRequest model)
        {
            var resultado = await _emprestimoService.CriarEmprestimoAsync(model);

            if (!resultado.Sucesso)
            {
                return BadRequest(new ProblemDetails { Title = "Empréstimo inválido", Detail = resultado.Erro });
            }

            return CreatedAtAction("BuscarPorId", new { id = resultado.Valor!.Id }, resultado.Valor);
        }


        [HttpGet("{id}", Name = "BuscarEmprestimoPorId")]
        public async Task<ActionResult> BuscarPorIdAsync(int id)
        {

            var resultado = await _emprestimoService.BuscarPorIdAsync(id);

            if (!resultado.Sucesso)
                return NotFound(new ProblemDetails { Title = "Empréstimo não encontrado", Detail = resultado.Erro });

            return Ok(resultado.Valor);

        }


        [HttpGet("ativos")]
        public async Task<ActionResult> ListarEmprestimosAtivosAsync()
        {
            var resultado = await _emprestimoService.ListarEmprestimosAtivosAsync();

            return Ok(resultado);
        }


        [HttpGet("atrasados")]
        public async Task<ActionResult> ListarEmprestimosAtrasadosAsync()
        {
            var resultado = await _emprestimoService.ListarEmprestimosAtrasadosAsync();

            return Ok(resultado);
        }


        [HttpPut("{id}/devolver")]
        public async Task<ActionResult> DevolverLivroAsync(int id)
        {
            var resultado = await _emprestimoService.DevolverLivroAsync(id);

            if (!resultado.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Não foi possível devolver o livro",
                    Detail = resultado.Erro
                });
            }
            return NoContent();


        }

    }
}
