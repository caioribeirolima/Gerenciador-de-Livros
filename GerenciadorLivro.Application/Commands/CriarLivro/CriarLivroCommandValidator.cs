using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroCommandValidator : AbstractValidator<CriarLivroCommand>
    {
        public CriarLivroCommandValidator(TimeProvider timeProvider)
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório.")
                .MaximumLength(200).WithMessage("O título do livro não pode exceder 200 caracteres.");

            RuleFor(x => x.AnoPublicacao)
                .LessThanOrEqualTo(_ => timeProvider.GetLocalNow().Year)
                .WithMessage("Ano de publicação não pode ser maior que o ano atual.");

            RuleForEach(x => x.AutorIds)
                .GreaterThan(0).WithMessage("O Id de um dos autores é inválido.");
            RuleFor(x => x.AutorIds)
                .Must(x => x.Distinct().Count() == x.Count).WithMessage("Não é permitido autores duplicados.");

            RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("A categoria do livro é obrigatória.");
            RuleFor(x => x.EditoraId).GreaterThan(0).WithMessage("A editora do livro é obrigatória.");
        }
    }
}
