using FluentValidation;

namespace GerenciadorLivro.Application.Commands.RenovarEmprestimo
{
    public class RenovarEmprestimoCommandValidator : AbstractValidator<RenovarEmprestimoCommand>
    {
        public RenovarEmprestimoCommandValidator()
        {
            RuleFor(x => x.EmprestimoId)
                .GreaterThan(0).WithMessage("O Id do empréstimo deve ser maior que zero.");
        }
    }
}
