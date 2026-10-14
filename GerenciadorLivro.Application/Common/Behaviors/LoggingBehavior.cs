using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GerenciadorLivro.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var nome = typeof(TRequest).Name;
            _logger.LogInformation("Iniciando {Request}", nome);

            var inicio = Stopwatch.GetTimestamp();
            try
            {
                var resposta = await next();
                _logger.LogInformation("Concluído {Request} em {Ms} ms",
                    nome, Stopwatch.GetElapsedTime(inicio).TotalMilliseconds);
                return resposta;
            }
            catch (Exception)
            {
                _logger.LogWarning("Falha em {Request} após {Ms} ms",
                    nome, Stopwatch.GetElapsedTime(inicio).TotalMilliseconds);
                throw;
            }
        }
    }
}
