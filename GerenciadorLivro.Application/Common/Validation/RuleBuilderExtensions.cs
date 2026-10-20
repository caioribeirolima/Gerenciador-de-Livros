using FluentValidation;

namespace GerenciadorLivro.Application.Common.Validation
{
    public static class RuleBuilderExtensions
    {
        // NotEmpty já rejeita só espaços (usa IsNullOrWhiteSpace): confirmado com "   " em teste, que cai em "é obrigatório".
        public static IRuleBuilderOptions<T, string> NomeObrigatorio<T>(this IRuleBuilder<T, string> rule, int max, string campo)
            => rule
                .NotEmpty().WithMessage($"O {campo} é obrigatório.")
                .MaximumLength(max).WithMessage($"O {campo} deve ter no máximo {max} caracteres.");

        // Cascade(Stop): CPF vazio recebe só "obrigatório", não "obrigatório" + "inválido".
        // A mensagem não repete o valor informado: CPF é dado pessoal e não deve circular em respostas e logs.
        public static IRuleBuilderOptions<T, string> CpfObrigatorio<T>(this IRuleBuilderInitial<T, string> rule)
            => rule
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Must(Cpf.EhValido).WithMessage("O CPF informado é inválido.");
    }
}
