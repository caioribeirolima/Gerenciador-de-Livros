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
    }
}
