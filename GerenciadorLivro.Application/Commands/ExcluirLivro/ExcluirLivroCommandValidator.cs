using FluentValidation;

namespace GerenciadorLivro.Application.Commands.ExcluirLivro
{
    public class ExcluirLivroCommandValidator : AbstractValidator<ExcluirLivroCommand>
    {
        public ExcluirLivroCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O Id do livro deve ser maior que zero.");
        }
    }
}
