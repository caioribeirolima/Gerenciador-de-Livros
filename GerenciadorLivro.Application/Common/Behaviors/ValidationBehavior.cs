using FluentValidation;
using MediatR;

namespace GerenciadorLivro.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull 
        where TResponse : IFalhaDeValidacao<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);
            var resultados = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
            var falhas = resultados.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

            if (falhas.Count != 0)
            {
                var erros = falhas
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

                return TResponse.Validacao(erros);
            }
            return await next();
        }
    }
}
