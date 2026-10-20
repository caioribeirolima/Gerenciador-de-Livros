using GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro;
using GerenciadorLivro.Application.Commands.CancelarReserva;
using GerenciadorLivro.Application.Commands.DevolverEmprestimo;
using GerenciadorLivro.Application.Commands.ReservarLivro;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Common
{
    public class ValidacaoTests
    {
        private class UnitOfWorkEspiao : IUnitOfWork
        {
            public int Commits { get; private set; }

            public Task<int> CommitAsync(CancellationToken cancellationToken)
            {
                Commits++;
                return Task.FromResult(0);
            }
        }

        [Theory]
        [InlineData(nameof(AdicionarAutorAoLivroCommand), 0)]
        [InlineData(nameof(CancelarReservaCommand), 0)]
        [InlineData(nameof(DevolverEmprestimoCommand), -1)]
        [InlineData(nameof(ReservarLivroCommand), 0)]
        public async Task IdInvalido_RetornaValidacaoSemGravar(string comando, int id)
        {
            var espiao = new UnitOfWorkEspiao();
            using var sp = Criar(s => s.AddSingleton<IUnitOfWork>(espiao));
            using var scope = sp.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var tipo = comando switch
            {
                nameof(AdicionarAutorAoLivroCommand) => (await mediator.Send(new AdicionarAutorAoLivroCommand { AutorId = id, LivroId = id })).Tipo,
                nameof(CancelarReservaCommand) => (await mediator.Send(new CancelarReservaCommand { ReservaId = id })).Tipo,
                nameof(DevolverEmprestimoCommand) => (await mediator.Send(new DevolverEmprestimoCommand { EmprestimoId = id })).Tipo,
                nameof(ReservarLivroCommand) => (await mediator.Send(new ReservarLivroCommand { LivroId = id, UsuarioId = id })).Tipo,
                _ => throw new ArgumentOutOfRangeException(nameof(comando), comando, null)
            };

            Assert.Equal(TipoErro.Validacao, tipo);
            Assert.Equal(0, espiao.Commits);
        }
    }
}
