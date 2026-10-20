using FluentValidation;

namespace GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro
{
    public class AdicionarAutorAoLivroCommandValidator : AbstractValidator<AdicionarAutorAoLivroCommand>
    {
        public AdicionarAutorAoLivroCommandValidator()
        {
            RuleFor(x => x.AutorId)
                .GreaterThan(0).WithMessage("O Id do autor deve ser maior que zero.");
            RuleFor(x => x.LivroId)
                .GreaterThan(0).WithMessage("O Id do livro deve ser maior que zero.");
        }
    }
}
