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

        // Cada linha tem UM id inválido e o outro válido, para que o teste quebre se a regra daquele id sumir.
        // Adicionar: a = AutorId, b = LivroId. Reservar: a = LivroId, b = UsuarioId. Cancelar e Devolver usam só "a".
        [Theory]
        [InlineData(nameof(AdicionarAutorAoLivroCommand), 0, 1)]
        [InlineData(nameof(AdicionarAutorAoLivroCommand), 1, 0)]
        [InlineData(nameof(CancelarReservaCommand), 0, 1)]
        [InlineData(nameof(CancelarReservaCommand), -1, 1)]
        [InlineData(nameof(DevolverEmprestimoCommand), 0, 1)]
        [InlineData(nameof(DevolverEmprestimoCommand), -1, 1)]
        [InlineData(nameof(ReservarLivroCommand), 0, 1)]
        [InlineData(nameof(ReservarLivroCommand), 1, 0)]
        [InlineData(nameof(ReservarLivroCommand), -5, 1)]
        public async Task IdInvalido_RetornaValidacaoSemGravar(string comando, int a, int b)
        {
            var espiao = new UnitOfWorkEspiao();
            using var sp = Criar(s => s.AddSingleton<IUnitOfWork>(espiao));
            using var scope = sp.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var tipo = comando switch
            {
                nameof(AdicionarAutorAoLivroCommand) => (await mediator.Send(new AdicionarAutorAoLivroCommand { AutorId = a, LivroId = b })).Tipo,
                nameof(CancelarReservaCommand) => (await mediator.Send(new CancelarReservaCommand { ReservaId = a })).Tipo,
                nameof(DevolverEmprestimoCommand) => (await mediator.Send(new DevolverEmprestimoCommand { EmprestimoId = a })).Tipo,
                nameof(ReservarLivroCommand) => (await mediator.Send(new ReservarLivroCommand { LivroId = a, UsuarioId = b })).Tipo,
                _ => throw new ArgumentOutOfRangeException(nameof(comando), comando, null)
            };

            Assert.Equal(TipoErro.Validacao, tipo);
            Assert.Equal(0, espiao.Commits);
        }

        [Fact]
        public async Task ReservarLivro_LivroIdZero_ReportaUmaUnicaMensagem()
        {
            using var sp = Criar();
            using var scope = sp.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new ReservarLivroCommand { LivroId = 0, UsuarioId = 1 });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.Single(result.ErrosValidacao![nameof(ReservarLivroCommand.LivroId)]);
        }
    }
}
