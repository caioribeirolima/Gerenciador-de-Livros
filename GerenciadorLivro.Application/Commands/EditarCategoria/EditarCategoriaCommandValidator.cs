using FluentValidation;
using GerenciadorLivro.Application.Common.Validation;

namespace GerenciadorLivro.Application.Commands.EditarCategoria
{
    public class EditarCategoriaCommandValidator : AbstractValidator<EditarCategoriaCommand>
    {
        public EditarCategoriaCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID da categoria é obrigatório.");

            RuleFor(x => x.Nome).NomeObrigatorio(100, "nome da categoria");
        }
    }
}
