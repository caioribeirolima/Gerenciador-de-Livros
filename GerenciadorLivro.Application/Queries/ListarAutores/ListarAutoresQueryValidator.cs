using FluentValidation;

namespace GerenciadorLivro.Application.Queries.ListarAutores
{
    public class ListarAutoresQueryValidator : AbstractValidator<ListarAutoresQuery>
    {
        public ListarAutoresQueryValidator()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(100).WithMessage("O nome do autor não pode exceder 100 caracteres.");

            RuleFor(x => x.Pagina)
                .GreaterThanOrEqualTo(1).WithMessage("O número da página deve ser maior ou igual a 1.");

            RuleFor(x => x.TamanhoPagina)
                .ExclusiveBetween(1, 50).WithMessage("O tamanho da página deve ser maior que 0 e menor que 100.");
        }
    }
}
