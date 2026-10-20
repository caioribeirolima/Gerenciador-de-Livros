using FluentValidation;
using GerenciadorLivro.Application.Common.Validation;

namespace GerenciadorLivro.Application.Commands.CriarEditora
{
    public class CriarEditoraCommandValidator : AbstractValidator<CriarEditoraCommand>
    {
        public CriarEditoraCommandValidator()
        {
            RuleFor(x => x.Nome).NomeObrigatorio(100, "nome da editora");
        }
    }
}
