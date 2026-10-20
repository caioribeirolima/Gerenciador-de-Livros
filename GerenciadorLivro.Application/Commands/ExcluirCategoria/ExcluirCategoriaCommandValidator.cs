using FluentValidation;

namespace GerenciadorLivro.Application.Commands.ExcluirCategoria
{
    public class ExcluirCategoriaCommandValidator : AbstractValidator<ExcluirCategoriaCommand>
    {
        public ExcluirCategoriaCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O Id da categoria deve ser maior que zero.");
        }
    }
}
