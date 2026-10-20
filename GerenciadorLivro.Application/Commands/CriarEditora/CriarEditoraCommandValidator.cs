using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CriarEditora
{
    public class CriarEditoraCommandValidator : AbstractValidator<CriarEditoraCommand>
    {
        public CriarEditoraCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome da editora é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da editora deve ter no máximo 100 caracteres.");
        }
    }
}
