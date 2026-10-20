using GerenciadorLivro.Application.Commands.RenovarEmprestimo;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Infrastructure.Persisting;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Emprestimos
{
    public class RenovacaoTests
    {
        private static readonly DateTime Ontem = DateTime.Now.AddDays(-1);

        private static async Task<TResponse> EnviarAsync<TResponse>(ServiceProvider sp, IRequest<TResponse> request)
        {
            using var scope = sp.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        private static async Task<ServiceProvider> CriarComEmprestimoAsync(Action<Emprestimo>? ajustar = null, Action<AppDbContext>? extras = null)
        {
            var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, false));
                db.Usuarios.AddRange(Usuario(1), Usuario(2));
                var emprestimo = EmprestimoAberto(1, 1, 1);
                ajustar?.Invoke(emprestimo);
                db.Emprestimos.Add(emprestimo);
                extras?.Invoke(db);
            });
            return sp;
        }

        private static Task<Emprestimo> LerEmprestimoAsync(ServiceProvider sp)
            => sp.LerAsync(db => db.Emprestimos.SingleAsync(e => e.Id == 1));

        // 1
        [Fact]
        public async Task Renovar_EmprestimoEmDia_SomaDiasADataPrevistaENaoAHoje()
        {
            using var sp = await CriarComEmprestimoAsync();
            var previstaAntes = (await LerEmprestimoAsync(sp)).DataDevolucaoPrevista;

            var result = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(result.Sucesso);
            var emprestimo = await LerEmprestimoAsync(sp);
            Assert.Equal(previstaAntes.AddDays(7), emprestimo.DataDevolucaoPrevista);
            Assert.Equal(1, emprestimo.Renovacoes);
        }

        // 2
        [Fact]
        public async Task Renovar_EmprestimoInexistenteOuDevolvido_Falha()
        {
            using var sp = await CriarComEmprestimoAsync(e => e.DataDevolucaoReal = DateTime.Now);

            var inexistente = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 99 });
            var devolvido = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.False(inexistente.Sucesso);
            Assert.Contains("não encontrado", inexistente.Erro);
            Assert.False(devolvido.Sucesso);
            Assert.Contains("já devolvido", devolvido.Erro);
        }

        // 3
        [Fact]
        public async Task Renovar_AteOLimite_TerceiraRenovacaoFalha()
        {
            using var sp = await CriarComEmprestimoAsync();

            var primeira = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });
            var segunda = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });
            var terceira = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(primeira.Sucesso);
            Assert.True(segunda.Sucesso);
            Assert.False(terceira.Sucesso);
            Assert.Contains("limite de renovações", terceira.Erro);
            Assert.Equal(2, (await LerEmprestimoAsync(sp)).Renovacoes);
        }

        // 4
        [Fact]
        public async Task Renovar_VenceuOntem_Falha()
        {
            using var sp = await CriarComEmprestimoAsync(e =>
            {
                e.DataEmprestimo = DateTime.Now.AddDays(-10);
                e.DataDevolucaoPrevista = Ontem;
            });

            var result = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.False(result.Sucesso);
            Assert.Contains("atrasado", result.Erro);
        }

        // 5
        [Fact]
        public async Task Renovar_VenceHojeEmHorarioJaPassado_NaoContaComoAtraso()
        {
            // Meia-noite de hoje já passou, mas a regra compara só a data.
            using var sp = await CriarComEmprestimoAsync(e =>
            {
                e.DataEmprestimo = DateTime.Today.AddDays(-7);
                e.DataDevolucaoPrevista = DateTime.Today;
            });

            var result = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(result.Sucesso);
            Assert.Equal(DateTime.Today.AddDays(7), (await LerEmprestimoAsync(sp)).DataDevolucaoPrevista);
        }

        // 6
        [Fact]
        public async Task Renovar_ComReservaAtivaOuNotificadaDeOutroUsuario_FalhaENaoAlteraEmprestimo()
        {
            using var comAtiva = await CriarComEmprestimoAsync(extras: db => db.Reservas.Add(Reserva(1, 1, 2, Ontem)));
            using var comNotificada = await CriarComEmprestimoAsync(extras: db => db.Reservas.Add(Reserva(1, 1, 2, Ontem, StatusReserva.Notificada)));
            var previstaAntes = (await LerEmprestimoAsync(comAtiva)).DataDevolucaoPrevista;

            var ativa = await EnviarAsync(comAtiva, new RenovarEmprestimoCommand { EmprestimoId = 1 });
            var notificada = await EnviarAsync(comNotificada, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.False(ativa.Sucesso);
            Assert.Contains("reserva de outro usuário", ativa.Erro);
            Assert.False(notificada.Sucesso);
            Assert.Contains("reserva de outro usuário", notificada.Erro);
            var emprestimo = await LerEmprestimoAsync(comAtiva);
            Assert.Equal(previstaAntes, emprestimo.DataDevolucaoPrevista);
            Assert.Equal(0, emprestimo.Renovacoes);
        }

        // 7
        [Fact]
        public async Task Renovar_ReservaCanceladaAtendidaOuDoProprioUsuario_NaoBloqueia()
        {
            using var sp = await CriarComEmprestimoAsync(extras: db =>
            {
                db.Reservas.Add(Reserva(1, 1, 2, Ontem, StatusReserva.Cancelada));
                db.Reservas.Add(Reserva(2, 1, 2, Ontem, StatusReserva.Atendida));
                db.Reservas.Add(Reserva(3, 1, 1, Ontem, StatusReserva.Ativa));
            });

            var result = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(result.Sucesso);
        }

        // 8
        [Fact]
        public async Task Renovar_LivroExcluidoComEmprestimoAberto_FalhaSemAlterarEmprestimo()
        {
            using var sp = await CriarComEmprestimoAsync(extras: db => db.Livros.Local.Single(l => l.Id == 1).Excluido = true);

            var result = await EnviarAsync(sp, new RenovarEmprestimoCommand { EmprestimoId = 1 });

            Assert.False(result.Sucesso);
            Assert.Contains("excluído do acervo", result.Erro);
            var emprestimo = await LerEmprestimoAsync(sp);
            Assert.Equal(0, emprestimo.Renovacoes);
            Assert.Null(emprestimo.DataDevolucaoReal);
        }

        [Fact]
        public void Validacao_GuardaTipoEErrosPorCampo()
        {
            var erros = new Dictionary<string, string[]> { ["Titulo"] = new[] { "obrigatório" } };

            var result = Result<int>.Validacao(erros);

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.Equal("obrigatório", result.ErrosValidacao!["Titulo"][0]);
        }
    }
}
