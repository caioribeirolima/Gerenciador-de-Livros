using FluentValidation;
using GerenciadorLivro.Application.Common.Validation;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaCommandValidator : AbstractValidator<CriarCategoriaCommand>
    {
        public CriarCategoriaCommandValidator()
        {
            RuleFor(x => x.Nome).NomeObrigatorio(100, "nome da categoria");
        }
    }
}
