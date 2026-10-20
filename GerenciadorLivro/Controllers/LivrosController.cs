using GerenciadorLivro.API.Extensions;
using GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro;
using GerenciadorLivro.Application.Commands.CriarLivro;
using GerenciadorLivro.Application.Commands.EditarLivro;
using GerenciadorLivro.Application.Commands.ExcluirLivro;
using GerenciadorLivro.Application.DTOs.InputModels;
using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Application.Queries.ListarLivros;
using GerenciadorLivro.Application.Queries.ListarLivrosPorAno;
using GerenciadorLivro.Application.Queries.ObterLivroDetalhes;
using GerenciadorLivro.Application.Queries.ObterLivroPorId;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly EmprestimoSettings _emprestimoSettings;
        private readonly UploadSettings _uploadSettings;
        private readonly IMediator _mediator;

        public LivrosController(
            IOptions<EmprestimoSettings> emprestimoSettings,
            IOptions<UploadSettings> uploadSettings,
            IMediator mediator)
        {
            _emprestimoSettings = emprestimoSettings.Value;
            _uploadSettings = uploadSettings.Value;
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ActionResult<List<LivroResponse>>> ListarTodosAsync(
            [FromQuery] string? autor,
            [FromQuery] bool? disponivel,
            [FromQuery] int? categoria,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            var result = await _mediator.Send(new ListarLivrosQuery
            {
                Autor = autor,
                Disponivel = disponivel,
                Categoria = categoria,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            });

            return Ok(result.Valor);
        }


        [HttpGet("{id}", Name = "ObterLivroPorId")]
        public async Task<ActionResult> BuscarPorIdAsync(int id)
        {
            var result = await _mediator.Send(new ObterLivroPorIdQuery { Id = id });

            if (!result.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Livro não encontrado",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Valor);
        }


        [HttpGet("{id}/detalhes", Name = "ObterLivroPorIdComDetalhes")]
        public async Task<ActionResult> BuscarPorIdComDetalhesAsync(int id)
        {
            var result = await _mediator.Send(new ObterLivroDetalhesQuery { Id = id });

            if (!result.Sucesso)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Livro não encontrado",
                    Detail = result.Erro,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(result.Valor);
        }


        [HttpGet("ano/{ano}")]
        public async Task<ActionResult<List<LivroResponse>>> ListarPorAnoAsync(int ano)
        {
            var result = await _mediator.Send(new ListarLivrosPorAnoQuery { Ano = ano });
            return Ok(result.Valor);
        }


        [HttpPost]
        public async Task<ActionResult> CriarAsync(CriarLivroRequest request)
        {
            var command = new CriarLivroCommand
            {
                Titulo = request.Titulo,
                AnoPublicacao = request.AnoPublicacao,
                CategoriaId = request.CategoriaId,
                EditoraId = request.EditoraId,
                AutorIds = request.AutorIds
            };

            var result = await _mediator.Send(command);

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na criação do livro", result.ErrosValidacao);

            return CreatedAtRoute("ObterLivroPorId", new { id = result.Valor }, new { id = result.Valor });
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> EditarAsync(int id, EditarLivroRequest request)
        {
            var result = await _mediator.Send(new EditarLivroCommand
            {
                Id = id,
                Titulo = request.Titulo,
                AnoPublicacao = request.AnoPublicacao,
                CategoriaId = request.CategoriaId,
                EditoraId = request.EditoraId
            });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Erro na edição do livro", result.ErrosValidacao);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirAsync(int id)
        {
            var result = await _mediator.Send(new ExcluirLivroCommand { Id = id });

            if (!result.Sucesso)
                return this.ErroDe(result.Tipo, result.Erro, "Livro não encontrado");

            return NoContent();
        }


        [HttpPost("{livroId}/autores/{autorId}")]
        public async Task<ActionResult> AdicionarAutorAsync(int livroId, int autorId)
        {
            var result = await _mediator.Send(new AdicionarAutorAoLivroCommand
            {
                LivroId = livroId,
                AutorId = autorId
            });

            if (!result.Sucesso)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Erro ao adicionar autor",
                    Detail = result.Erro,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return NoContent();
        }


        [HttpPost("upload-capa/{livroId}")]
        public async Task<ActionResult> UploadArquivo(int livroId, IFormFile capa)
        {
            if (capa == null || capa.Length == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nenhum arquivo enviado"
                });
            }

            var extensao = Path
                .GetExtension(capa.FileName)
                .ToLowerInvariant();

            if (!_uploadSettings.ExtensoesPermitidas
                .Contains(extensao, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Tipo de arquivo não permitido",
                    Detail = $"Extensões permitidas: {string.Join(", ", _uploadSettings.ExtensoesPermitidas)}"
                });
            }

            var tamanhoMaximoBytes =
                _uploadSettings.TamanhoMaximoMB * 1024L * 1024L;
            if (capa.Length > tamanhoMaximoBytes)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Arquivo excede o tamanho máximo",
                    Detail = $"O tamanho máximo permitido é {_uploadSettings.TamanhoMaximoMB} MB."
                });
            }

            var nomeArquivo = $"{livroId}_{capa.FileName}";
            var pasta = Path.Combine("Uploads", "Livros");

            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }

            var caminho = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await capa.CopyToAsync(stream);
            }

            return Ok(new
            {
                mensagem = "Upload concluído",
                nome = capa.FileName,
                tamanho = capa.Length
            });
        }

    }
}
