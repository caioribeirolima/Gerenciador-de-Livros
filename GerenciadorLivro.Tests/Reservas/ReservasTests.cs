using GerenciadorLivro.Application.Commands.CancelarReserva;
using GerenciadorLivro.Application.Commands.DevolverEmprestimo;
using GerenciadorLivro.Application.Commands.RegistrarEmprestimo;
using GerenciadorLivro.Application.Commands.ReservarLivro;
using GerenciadorLivro.Application.Notifications.Reservas;
using GerenciadorLivro.Application.Queries.ListarFilaDoLivro;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Reservas
{
    public class ReservasTests
    {
        private static readonly DateTime Ontem = DateTime.Now.AddDays(-1);

        private static async Task<TResponse> EnviarAsync<TResponse>(ServiceProvider sp, IRequest<TResponse> request)
        {
            using var scope = sp.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        // ---------- Reservar ----------

        [Fact]
        public async Task Reservar_LivroOuUsuarioInexistente_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => { db.Livros.Add(Livro(1, false)); db.Usuarios.Add(Usuario(1)); });

            var semLivro = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 99, UsuarioId = 1 });
            var semUsuario = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 1, UsuarioId = 99 });

            Assert.False(semLivro.Sucesso);
            Assert.Contains("Livro de id 99", semLivro.Erro);
            Assert.False(semUsuario.Sucesso);
            Assert.Contains("Usuário de id 99", semUsuario.Erro);
        }

        [Fact]
        public async Task Reservar_LivroDisponivel_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => { db.Livros.Add(Livro(1, true)); db.Usuarios.Add(Usuario(1)); });

            var result = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 1, UsuarioId = 1 });

            Assert.False(result.Sucesso);
            Assert.Contains("disponível", result.Erro);
        }

        [Fact]
        public async Task Reservar_LivroIndisponivel_CriaReservaAtiva()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => { db.Livros.Add(Livro(1, false)); db.Usuarios.Add(Usuario(1)); });

            var result = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 1, UsuarioId = 1 });

            Assert.True(result.Sucesso);
            var reserva = await sp.LerAsync(db => db.Reservas.SingleAsync(r => r.Id == result.Valor));
            Assert.Equal(StatusReserva.Ativa, reserva.Status);
        }

        [Fact]
        public async Task Reservar_ComReservaAtivaOuNotificadaDuplicada_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.AddRange(Livro(1, false), Livro(2, true));
                db.Usuarios.AddRange(Usuario(1), Usuario(2));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem, StatusReserva.Ativa));
                db.Reservas.Add(Reserva(2, 2, 2, Ontem, StatusReserva.Notificada));
            });

            var duplicadaAtiva = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 1, UsuarioId = 1 });
            var duplicadaNotificada = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 2, UsuarioId = 2 });

            Assert.False(duplicadaAtiva.Sucesso);
            Assert.Contains("reserva pendente", duplicadaAtiva.Erro);
            Assert.False(duplicadaNotificada.Sucesso);
        }

        [Fact]
        public async Task Reservar_UsuarioJaEstaComOLivro_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, false));
                db.Usuarios.Add(Usuario(1));
                db.Emprestimos.Add(EmprestimoAberto(1, 1, 1));
            });

            var result = await EnviarAsync(sp, new ReservarLivroCommand { LivroId = 1, UsuarioId = 1 });

            Assert.False(result.Sucesso);
            Assert.Contains("emprestado", result.Erro);
        }

        // ---------- Cancelar ----------

        [Fact]
        public async Task Cancelar_ReservaInexistenteOuComStatusInvalido_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, false));
                db.Usuarios.Add(Usuario(1));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem, StatusReserva.Atendida));
                db.Reservas.Add(Reserva(2, 1, 1, Ontem, StatusReserva.Cancelada));
                db.Reservas.Add(Reserva(3, 1, 1, Ontem, StatusReserva.Ativa));
            });

            var inexistente = await EnviarAsync(sp, new CancelarReservaCommand { ReservaId = 99 });
            var atendida = await EnviarAsync(sp, new CancelarReservaCommand { ReservaId = 1 });
            var cancelada = await EnviarAsync(sp, new CancelarReservaCommand { ReservaId = 2 });
            var ativa = await EnviarAsync(sp, new CancelarReservaCommand { ReservaId = 3 });

            Assert.False(inexistente.Sucesso);
            Assert.False(atendida.Sucesso);
            Assert.False(cancelada.Sucesso);
            Assert.True(ativa.Sucesso);
            var status = await sp.LerAsync(db => db.Reservas.Where(r => r.Id == 3).Select(r => r.Status).SingleAsync());
            Assert.Equal(StatusReserva.Cancelada, status);
        }

        // ---------- Devolução ----------

        [Fact]
        public async Task Devolver_PrimeiraAtivaPorDataReservaViraNotificada()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, false));
                db.Usuarios.AddRange(Usuario(1), Usuario(2), Usuario(3), Usuario(4));
                db.Emprestimos.Add(EmprestimoAberto(1, 1, 1));
                // Ids fora da ordem de data, para provar que a ordem vem de DataReserva.
                db.Reservas.Add(Reserva(10, 1, 2, Ontem.AddHours(2)));
                db.Reservas.Add(Reserva(11, 1, 3, Ontem));            // mais antiga
                db.Reservas.Add(Reserva(12, 1, 4, Ontem.AddHours(1)));
            });

            var result = await EnviarAsync(sp, new DevolverEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(result.Sucesso);
            var reservas = await sp.LerAsync(db => db.Reservas.OrderBy(r => r.Id).ToListAsync());
            Assert.Equal(StatusReserva.Ativa, reservas[0].Status);
            Assert.Equal(StatusReserva.Notificada, reservas[1].Status);
            Assert.NotNull(reservas[1].DataNotificacao);
            Assert.Equal(StatusReserva.Ativa, reservas[2].Status);
        }

        [Fact]
        public async Task CancelarNotificada_PassaOLivroParaAProximaDaFila()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, true));
                db.Usuarios.AddRange(Usuario(1), Usuario(2));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem, StatusReserva.Notificada));
                db.Reservas.Add(Reserva(2, 1, 2, Ontem.AddHours(1)));
            });

            var result = await EnviarAsync(sp, new CancelarReservaCommand { ReservaId = 1 });

            Assert.True(result.Sucesso);
            var status = await sp.LerAsync(db => db.Reservas.Where(r => r.Id == 2).Select(r => r.Status).SingleAsync());
            Assert.Equal(StatusReserva.Notificada, status);
        }

        [Fact]
        public async Task Devolver_LivroExcluidoComEmprestimoAberto_DevolucaoFunciona()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                var livro = Livro(1, false);
                livro.Excluido = true; // soft delete: o filtro global esconde o livro das consultas comuns
                db.Livros.Add(livro);
                db.Usuarios.Add(Usuario(1));
                db.Emprestimos.Add(EmprestimoAberto(1, 1, 1));
            });

            var result = await EnviarAsync(sp, new DevolverEmprestimoCommand { EmprestimoId = 1 });

            Assert.True(result.Sucesso);
            var devolvido = await sp.LerAsync(db => db.Emprestimos.Where(e => e.Id == 1).Select(e => e.DataDevolucaoReal).SingleAsync());
            Assert.NotNull(devolvido);
        }

        // ---------- RegistrarEmprestimo ----------

        [Fact]
        public async Task RegistrarEmprestimo_SemReservas_ContinuaFuncionando()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => { db.Livros.Add(Livro(1, true)); db.Usuarios.Add(Usuario(1)); });

            var result = await EnviarAsync(sp, new RegistrarEmprestimoCommand { LivroId = 1, UsuarioId = 1 });

            Assert.True(result.Sucesso);
            var livroDisponivel = await sp.LerAsync(db => db.Livros.Where(l => l.Id == 1).Select(l => l.Disponivel).SingleAsync());
            Assert.False(livroDisponivel);
        }

        [Fact]
        public async Task RegistrarEmprestimo_ComNotificadaDeOutroUsuario_Bloqueia()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, true));
                db.Usuarios.AddRange(Usuario(1), Usuario(2));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem, StatusReserva.Notificada));
            });

            var result = await EnviarAsync(sp, new RegistrarEmprestimoCommand { LivroId = 1, UsuarioId = 2 });

            Assert.False(result.Sucesso);
            Assert.Contains("reservado para outro usuário", result.Erro);
            var emprestimos = await sp.LerAsync(db => db.Emprestimos.CountAsync());
            Assert.Equal(0, emprestimos);
        }

        [Fact]
        public async Task RegistrarEmprestimo_PeloDonoDaReserva_ReservaViraAtendida()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, true));
                db.Usuarios.Add(Usuario(1));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem, StatusReserva.Notificada));
            });

            var result = await EnviarAsync(sp, new RegistrarEmprestimoCommand { LivroId = 1, UsuarioId = 1 });

            Assert.True(result.Sucesso);
            var reserva = await sp.LerAsync(db => db.Reservas.SingleAsync(r => r.Id == 1));
            Assert.Equal(StatusReserva.Atendida, reserva.Status);
            Assert.NotNull(reserva.DataEncerramento);
        }

        // ---------- Query e nota de design ----------

        [Fact]
        public async Task ListarFila_CalculaPosicao_NotificadaPrimeiroDepoisAtivasPorData()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, true));
                db.Usuarios.AddRange(Usuario(1), Usuario(2), Usuario(3), Usuario(4));
                db.Reservas.Add(Reserva(1, 1, 1, Ontem.AddHours(3)));
                db.Reservas.Add(Reserva(2, 1, 2, Ontem, StatusReserva.Notificada));
                db.Reservas.Add(Reserva(3, 1, 3, Ontem.AddHours(1)));
                db.Reservas.Add(Reserva(4, 1, 4, Ontem, StatusReserva.Cancelada));
            });

            var fila = await EnviarAsync(sp, new ListarFilaDoLivroQuery { LivroId = 1 });

            Assert.Equal(3, fila.Count);
            Assert.Equal(new[] { 2, 3, 1 }, fila.Select(r => r.Id).ToArray());
            Assert.Equal(new int?[] { 1, 2, 3 }, fila.Select(r => r.Posicao).ToArray());
        }

        private class AvisoQueFalha : INotificationHandler<ReservaNotificadaNotification>
        {
            public Task Handle(ReservaNotificadaNotification notification, CancellationToken cancellationToken)
                => throw new InvalidOperationException("Servidor de e-mail fora do ar");
        }

        [Fact]
        public async Task Devolver_QuandoAvisoFalha_DevolucaoEReservaNotificadaJaEstaoGravadas()
        {
            using var sp = Criar(s => s.AddTransient<INotificationHandler<ReservaNotificadaNotification>, AvisoQueFalha>());
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, false));
                db.Usuarios.AddRange(Usuario(1), Usuario(2));
                db.Emprestimos.Add(EmprestimoAberto(1, 1, 1));
                db.Reservas.Add(Reserva(1, 1, 2, Ontem));
            });

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => EnviarAsync(sp, new DevolverEmprestimoCommand { EmprestimoId = 1 }));

            var devolvido = await sp.LerAsync(db => db.Emprestimos.Where(e => e.Id == 1).Select(e => e.DataDevolucaoReal).SingleAsync());
            var status = await sp.LerAsync(db => db.Reservas.Where(r => r.Id == 1).Select(r => r.Status).SingleAsync());
            Assert.NotNull(devolvido);
            Assert.Equal(StatusReserva.Notificada, status);
        }
    }
}
