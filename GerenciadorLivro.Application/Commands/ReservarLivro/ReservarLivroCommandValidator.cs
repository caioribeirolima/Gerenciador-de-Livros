using FluentValidation;

namespace GerenciadorLivro.Application.Commands.ReservarLivro
{
    public class ReservarLivroCommandValidator : AbstractValidator<ReservarLivroCommand>
    {
        public ReservarLivroCommandValidator()
        {
            RuleFor(x => x.LivroId)
                .NotEmpty().WithMessage("O Id do livro é obrigatório.")
                .GreaterThan(0).WithMessage("O Id do livro deve ser maior que zero.");
            RuleFor(x => x.UsuarioId)
                .NotEmpty().WithMessage("O Id do usuário é obrigatório.")
                .GreaterThan(0).WithMessage("O Id do usuário deve ser maior que zero.");
        }
    }
}
