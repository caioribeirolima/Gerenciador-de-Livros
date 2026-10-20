using GerenciadorLivro.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorLivro.API.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult ErroDe(
            this ControllerBase controller, 
            TipoErro? tipo, 
            string? detalhe, 
            string titulo, 
            IReadOnlyDictionary<string, string[]>? errosValidacao = null)
        {

            if (tipo == TipoErro.Validacao && errosValidacao is not null)
            {
                var problema = new ValidationProblemDetails(errosValidacao.ToDictionary(e => e.Key, e => e.Value))
                {
                    Title = "Erro de validação",
                    Status = StatusCodes.Status400BadRequest
                };
                return controller.ValidationProblem(problema);
            }

            var status = tipo switch
            {
                TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
                TipoErro.Conflito => StatusCodes.Status409Conflict,
                TipoErro.Validacao => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            return controller.Problem(title: titulo, detail: detalhe, statusCode: status);
        }
    }
}