using FluentValidation;
using GerenciadorLivro.Application.Common.Validation;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorCommandValidator : AbstractValidator<CriarAutorCommand>
    {
        public CriarAutorCommandValidator()
        {
            RuleFor(x => x.Nome).NomeObrigatorio(150, "nome do autor");
        }
    }
}
