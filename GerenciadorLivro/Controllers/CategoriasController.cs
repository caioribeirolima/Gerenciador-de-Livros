using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult> ListarTodosAsync()
            => Ok(await _categoriaService.ListarTodosAsync());

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarCategoriaRequest request)
        {
            var categoria = await _categoriaService.CriarAsync(request);
            return CreatedAtAction(nameof(ListarTodosAsync), new { id = categoria.Id }, categoria);
        }
    }
}
