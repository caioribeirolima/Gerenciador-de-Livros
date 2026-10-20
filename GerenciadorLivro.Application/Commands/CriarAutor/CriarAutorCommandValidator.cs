using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorCommandValidator : AbstractValidator<CriarAutorCommand>
    {
        public CriarAutorCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do autor é obrigatório.")
                .MaximumLength(150).WithMessage("O nome do autor deve ter no máximo 150 caracteres.");
        }
    }
}
