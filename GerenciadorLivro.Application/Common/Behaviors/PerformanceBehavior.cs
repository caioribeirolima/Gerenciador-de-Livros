using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GerenciadorLivro.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly PerformanceSettings _settings;

        public PerformanceBehavior(
            ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
            IOptions<PerformanceSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var inicio = Stopwatch.GetTimestamp();
            var resposta = await next();
            var decorridoMs = Stopwatch.GetElapsedTime(inicio).TotalMilliseconds;

            if (decorridoMs > _settings.LimiteMs)
            {
                _logger.LogWarning("Request lenta: {Request} levou {Ms} ms (limite {Limite} ms)",
                    typeof(TRequest).Name, decorridoMs, _settings.LimiteMs);
            }

            return resposta;
        }
    }
}
