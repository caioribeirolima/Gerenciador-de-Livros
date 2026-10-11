using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorasController : ControllerBase
    {
        private readonly IEditoraService _editoraService;

        public EditorasController(IEditoraService editoraService)
        {
            _editoraService = editoraService;
        }

        [HttpGet]
        public async Task<ActionResult> ListarTodosAsync()
            => Ok(await _editoraService.ListarTodosAsync());

        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarEditoraRequest request)
        {
            var editora = await _editoraService.CriarAsync(request);
            return CreatedAtAction(nameof(ListarTodosAsync), new { id = editora.Id }, editora);
        }
    }
}
