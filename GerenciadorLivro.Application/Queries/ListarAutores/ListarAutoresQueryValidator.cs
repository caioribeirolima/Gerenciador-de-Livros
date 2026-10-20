using FluentValidation;

namespace GerenciadorLivro.Application.Queries.ListarAutores
{
    public class ListarAutoresQueryValidator : AbstractValidator<ListarAutoresQuery>
    {
        public ListarAutoresQueryValidator()
        {
            // Mesmo limite do cadastro (CriarAutor = 150): com menos, não dá para buscar pelo nome completo
            // de um autor cujo nome tem entre 101 e 150 caracteres.
            RuleFor(x => x.Nome)
                .MaximumLength(150).WithMessage("O nome do autor não pode exceder 150 caracteres.");

            RuleFor(x => x.Pagina)
                .GreaterThanOrEqualTo(1).WithMessage("O número da página deve ser maior ou igual a 1.");

            RuleFor(x => x.TamanhoPagina)
                .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}
