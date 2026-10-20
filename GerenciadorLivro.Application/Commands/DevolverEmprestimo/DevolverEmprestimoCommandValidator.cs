using FluentValidation;

namespace GerenciadorLivro.Application.Commands.DevolverEmprestimo
{
    public class DevolverEmprestimoCommandValidator : AbstractValidator<DevolverEmprestimoCommand>
    {
        public DevolverEmprestimoCommandValidator()
        {
            RuleFor(x => x.EmprestimoId)
                .GreaterThan(0).WithMessage("O Id do empréstimo deve ser maior que zero.");
        }
    }
}
