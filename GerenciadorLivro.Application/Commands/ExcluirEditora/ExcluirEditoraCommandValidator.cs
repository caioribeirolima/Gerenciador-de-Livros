using FluentValidation;

namespace GerenciadorLivro.Application.Commands.ExcluirEditora
{
    public class ExcluirEditoraCommandValidator : AbstractValidator<ExcluirEditoraCommand>
    {
        public ExcluirEditoraCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O Id da editora deve ser maior que zero.");
        }
    }
}
