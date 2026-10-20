using FluentValidation;

namespace GerenciadorLivro.Application.Commands.RegistrarEmprestimo
{
    public class RegistrarEmprestimoCommandValidator : AbstractValidator<RegistrarEmprestimoCommand>
    {
        public RegistrarEmprestimoCommandValidator()
        {
            RuleFor(x => x.LivroId).GreaterThan(0).WithMessage("O ID do livro é obrigatório.");
            RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("O ID do usuário é obrigatório.");
        }
    }
}
