using FluentValidation;

namespace GerenciadorLivro.Application.Commands.EditarLivro
{
    public class EditarLivroCommandValidator : AbstractValidator<EditarLivroCommand>
    {
        public EditarLivroCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID do livro é obrigatório.");

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório.")
                .MaximumLength(200).WithMessage("O título do livro não pode exceder 200 caracteres.");

            RuleFor(x => x.AnoPublicacao)
                .LessThanOrEqualTo(_ => DateTime.Now.Year)
                .WithMessage("Ano de publicação não pode ser maior que o ano atual.");

            RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("A categoria do livro é obrigatória.");
            RuleFor(x => x.EditoraId).GreaterThan(0).WithMessage("A editora do livro é obrigatória.");
        }
    }
}
