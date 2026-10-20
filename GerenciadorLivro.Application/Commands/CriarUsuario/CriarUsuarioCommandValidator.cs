using FluentValidation;
using GerenciadorLivro.Application.Common.Validation;

namespace GerenciadorLivro.Application.Commands.CriarUsuario
{
    public class CriarUsuarioCommandValidator : AbstractValidator<CriarUsuarioCommand>
    {
        public CriarUsuarioCommandValidator()
        {
            RuleFor(x => x.Nome).NomeObrigatorio(150, "nome do usuário");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .MaximumLength(254).WithMessage("O e-mail deve ter no máximo 254 caracteres.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");

            RuleFor(x => x.Cpf).CpfObrigatorio();
        }
    }
}
