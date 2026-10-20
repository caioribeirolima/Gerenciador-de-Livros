using FluentValidation;

namespace GerenciadorLivro.Application.Commands.EditarCategoria
{
    public class EditarCategoriaCommandValidator : AbstractValidator<EditarCategoriaCommand>
    {
        public EditarCategoriaCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID da categoria é obrigatório.");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");
        }
    }
}
