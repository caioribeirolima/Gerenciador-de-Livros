using GerenciadorLivro.Application.Commands.CriarUsuario;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Queries.ObterUsuarioPorId;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Usuarios
{
    public class UsuariosTests
    {
        private const string CpfDigitos = "52998224725";
        private const string CpfMascara = "529.982.247-25";

        private static async Task<TResponse> EnviarAsync<TResponse>(ServiceProvider sp, IRequest<TResponse> request)
        {
            using var scope = sp.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        private static CriarUsuarioCommand NovoUsuario(string cpf = CpfMascara) => new()
        {
            Nome = "  Maria da Silva  ",
            Email = "maria@hospital.com.br",
            Cpf = cpf
        };

        // Simula o commit que falha por violação do índice único. Antes de falhar, executa "rival": o outro
        // request que gravou o mesmo CPF no intervalo entre a checagem do handler e o Commit.
        private class UnitOfWorkQueFalha : IUnitOfWork
        {
            private readonly Func<Task>? _rival;
            public UnitOfWorkQueFalha(Func<Task>? rival) => _rival = rival;

            public async Task<int> CommitAsync(CancellationToken cancellationToken)
            {
                if (_rival != null)
                    await _rival();

                throw new DbUpdateException("falha simulada no commit");
            }
        }

        // ---------- Criar ----------

        [Fact]
        public async Task Criar_ComSucesso_GravaSoOsDigitosDoCpf()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, NovoUsuario());

            Assert.True(result.Sucesso);
            var usuario = await sp.LerAsync(db => db.Usuarios.SingleAsync(u => u.Id == result.Valor));
            Assert.Equal(CpfDigitos, usuario.Cpf);
            Assert.Equal("Maria da Silva", usuario.Nome);
        }

        [Theory]
        [InlineData(CpfDigitos)]
        [InlineData(CpfMascara)]
        public async Task Criar_CpfJaCadastrado_ComOuSemMascara_Conflito(string cpfNovo)
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Usuarios.Add(new Usuario("Outro", "outro@x.com", CpfDigitos) { Id = 1 }));

            var result = await EnviarAsync(sp, NovoUsuario(cpfNovo));

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Conflito, result.Tipo);
            Assert.Equal(1, await sp.LerAsync(db => db.Usuarios.CountAsync()));
        }

        [Fact]
        public async Task Criar_CpfJaCadastrado_MensagemNaoVazaOCpf()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Usuarios.Add(new Usuario("Outro", "outro@x.com", CpfDigitos) { Id = 1 }));

            var result = await EnviarAsync(sp, NovoUsuario());

            Assert.DoesNotContain(CpfDigitos, result.Erro);
            Assert.DoesNotContain("982", result.Erro);
        }

        [Theory]
        [InlineData("52998224726")]
        [InlineData("11111111111")]
        [InlineData("")]
        public async Task Criar_CpfInvalido_RetornaValidacaoSemGravar(string cpf)
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, NovoUsuario(cpf));

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarUsuarioCommand.Cpf)));
            Assert.Equal(0, await sp.LerAsync(db => db.Usuarios.CountAsync()));
        }

        [Fact]
        public async Task Criar_VariosErros_ReportaTodosOsCamposDeUmaVez()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarUsuarioCommand { Nome = "", Email = "maria", Cpf = "123" });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.Contains(nameof(CriarUsuarioCommand.Nome), result.ErrosValidacao!.Keys);
            Assert.Contains(nameof(CriarUsuarioCommand.Email), result.ErrosValidacao.Keys);
            Assert.Contains(nameof(CriarUsuarioCommand.Cpf), result.ErrosValidacao.Keys);
        }

        [Fact]
        public async Task Criar_UsuariosAntigosSemCpf_NaoImpedemNovoCadastro()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Usuarios.Add(new Usuario("Antigo 1", "a1@x.com") { Id = 1 });
                db.Usuarios.Add(new Usuario("Antigo 2", "a2@x.com") { Id = 2 });
            });

            var result = await EnviarAsync(sp, NovoUsuario());

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Criar_OutroUsuarioGravaOMesmoCpfNoMeioDoCaminho_Conflito()
        {
            ServiceProvider? sp = null;
            sp = Criar(s => s.AddScoped<IUnitOfWork>(_ => new UnitOfWorkQueFalha(
                () => sp!.SemearAsync(db => db.Usuarios.Add(new Usuario("Rival", "rival@x.com", CpfDigitos) { Id = 50 })))));

            var result = await EnviarAsync(sp, NovoUsuario());

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Conflito, result.Tipo);
            // Só o rival ficou no banco: o usuário do handler foi retirado do contexto.
            Assert.Equal(1, await sp.LerAsync(db => db.Usuarios.CountAsync()));
            sp.Dispose();
        }

        [Fact]
        public async Task Criar_FalhaDoBancoSemRelacaoComCpf_RelancaAExcecao()
        {
            using var sp = Criar(s => s.AddScoped<IUnitOfWork>(_ => new UnitOfWorkQueFalha(null)));

            await Assert.ThrowsAsync<DbUpdateException>(() => EnviarAsync(sp, NovoUsuario()));
        }

        // ---------- Obter ----------

        [Fact]
        public async Task Obter_Existente_DevolveCpfMascarado()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Usuarios.Add(new Usuario("Maria", "maria@x.com", CpfDigitos) { Id = 1 }));

            var result = await EnviarAsync(sp, new ObterUsuarioPorIdQuery { Id = 1 });

            Assert.True(result.Sucesso);
            Assert.Equal("***.982.247-**", result.Valor!.CpfMascarado);
            Assert.Equal("Maria", result.Valor.Nome);
        }

        [Fact]
        public async Task Obter_UsuarioAntigoSemCpf_CpfMascaradoNulo()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Usuarios.Add(new Usuario("Antigo", "antigo@x.com") { Id = 1 }));

            var result = await EnviarAsync(sp, new ObterUsuarioPorIdQuery { Id = 1 });

            Assert.True(result.Sucesso);
            Assert.Null(result.Valor!.CpfMascarado);
        }

        [Fact]
        public async Task Obter_Inexistente_NaoEncontrado()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new ObterUsuarioPorIdQuery { Id = 99 });

            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
            Assert.Contains("Usuário de id 99", result.Erro);
        }

        [Fact]
        public async Task Obter_IdZero_RetornaValidacao()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new ObterUsuarioPorIdQuery { Id = 0 });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
        }
    }
}
