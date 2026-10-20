using FluentValidation;

namespace GerenciadorLivro.Application.Commands.ReservarLivro
{
    public class ReservarLivroCommandValidator : AbstractValidator<ReservarLivroCommand>
    {
        public ReservarLivroCommandValidator()
        {
            // Sem NotEmpty: para int ele só rejeita 0, e o GreaterThan(0) já cobre o 0.
            // Com os dois, LivroId = 0 recebia duas mensagens no mesmo campo.
            RuleFor(x => x.LivroId)
                .GreaterThan(0).WithMessage("O Id do livro deve ser maior que zero.");
            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("O Id do usuário deve ser maior que zero.");
        }
    }
}
