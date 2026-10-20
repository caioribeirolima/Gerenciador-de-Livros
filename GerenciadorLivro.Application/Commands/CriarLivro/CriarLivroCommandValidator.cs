using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroCommandValidator : AbstractValidator<CriarLivroCommand>
    {
        public CriarLivroCommandValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório.")
                .MaximumLength(100).WithMessage("O título do livro não pode exceder 100 caracteres.");

            RuleFor(x => x.AnoPublicacao)
                .LessThanOrEqualTo(_ => DateTime.Now.Year)
                .WithMessage("Ano de publicação não pode ser maior que o ano atual.");

            RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("A categoria do livro é obrigatória.");
            RuleFor(x => x.EditoraId).GreaterThan(0).WithMessage("A editora do livro é obrigatória.");
        }
    }
}
