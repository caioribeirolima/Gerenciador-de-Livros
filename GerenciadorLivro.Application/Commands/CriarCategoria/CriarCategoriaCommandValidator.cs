using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaCommandValidator : AbstractValidator<CriarCategoriaCommand>
    {
        public CriarCategoriaCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");
        }
    }
}
