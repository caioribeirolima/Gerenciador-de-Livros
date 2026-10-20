using FluentValidation;

namespace GerenciadorLivro.Application.Queries.ObterUsuarioPorId
{
    public class ObterUsuarioPorIdQueryValidator : AbstractValidator<ObterUsuarioPorIdQuery>
    {
        public ObterUsuarioPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O Id do usuário deve ser maior que zero.");
        }
    }
}
