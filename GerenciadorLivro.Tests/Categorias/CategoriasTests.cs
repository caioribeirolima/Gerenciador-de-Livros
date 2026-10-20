using GerenciadorLivro.Application.Commands.CriarCategoria;
using GerenciadorLivro.Application.Commands.EditarCategoria;
using GerenciadorLivro.Application.Commands.ExcluirCategoria;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Categorias
{
    public class CategoriasTests
    {
        private static Categoria Categoria(int id, string nome) =>
            new Categoria { Id = id, Nome = nome };

        private static async Task<TResponse> EnviarAsync<TResponse>(ServiceProvider sp, IRequest<TResponse> request)
        {
            using var scope = sp.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        // ---------- Criar ----------

        [Fact]
        public async Task Criar_ComSucesso()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarCategoriaCommand { Nome = "Ficção" });

            Assert.True(result.Sucesso);
            var nome = await sp.LerAsync(db =>
                db.Categorias.Where(c => c.Id == result.Valor).Select(c => c.Nome).SingleAsync());
            Assert.Equal("Ficção", nome);
        }

        [Fact]
        public async Task Criar_NomeDuplicado_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Categorias.Add(Categoria(1, "Ficção")));

            var result = await EnviarAsync(sp, new CriarCategoriaCommand { Nome = "Ficção" });

            Assert.False(result.Sucesso);
            Assert.Contains("já está cadastrada", result.Erro);
        }

        [Fact]
        public async Task Criar_NomeDuplicadoCaseDiferente_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Categorias.Add(Categoria(1, "Ficção")));

            var result = await EnviarAsync(sp, new CriarCategoriaCommand { Nome = "ficção" });

            Assert.False(result.Sucesso);
            Assert.Contains("já está cadastrada", result.Erro);
        }

        [Fact]
        public async Task Criar_NomeVazio_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarCategoriaCommand { Nome = "   " });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarCategoriaCommand.Nome)));
            var total = await sp.LerAsync(db => db.Categorias.CountAsync());
            Assert.Equal(0, total);
        }

        [Fact]
        public async Task Criar_NomeMaiorQue100_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarCategoriaCommand { Nome = new string('a', 101) });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            var total = await sp.LerAsync(db => db.Categorias.CountAsync());
            Assert.Equal(0, total);
        }

        // ---------- Editar ----------

        [Fact]
        public async Task Editar_MantenhaProprioNome_Sucesso()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Categorias.Add(Categoria(1, "Ficção")));

            var result = await EnviarAsync(sp, new EditarCategoriaCommand { Id = 1, Nome = "Ficção" });

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Editar_ParaNomeDeOutraCategoria_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Categorias.Add(Categoria(1, "Ficção"));
                db.Categorias.Add(Categoria(2, "Terror"));
            });

            var result = await EnviarAsync(sp, new EditarCategoriaCommand { Id = 1, Nome = "Terror" });

            Assert.False(result.Sucesso);
            Assert.Contains("Terror", result.Erro);
        }

        [Fact]
        public async Task Editar_NomeVazio_RetornaValidacaoSemAlterar()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Categorias.Add(Categoria(1, "Ficção")));

            var result = await EnviarAsync(sp, new EditarCategoriaCommand { Id = 1, Nome = "   " });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(EditarCategoriaCommand.Nome)));
            var nome = await sp.LerAsync(db => db.Categorias.Where(c => c.Id == 1).Select(c => c.Nome).SingleAsync());
            Assert.Equal("Ficção", nome);
        }

        // ---------- Excluir ----------

        [Fact]
        public async Task Excluir_ComLivroExcluidoVinculado_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Categorias.Add(Categoria(1, "Ficção"));
                var livro = Livro(1, false); // CategoriaId = 1
                livro.Excluido = true;
                db.Livros.Add(livro);
            });

            var result = await EnviarAsync(sp, new ExcluirCategoriaCommand { Id = 1 });

            Assert.False(result.Sucesso);
            Assert.Contains("livros vinculados", result.Erro);
        }

        [Fact]
        public async Task Excluir_ComSucesso()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Categorias.Add(Categoria(1, "Ficção")));

            var result = await EnviarAsync(sp, new ExcluirCategoriaCommand { Id = 1 });

            Assert.True(result.Sucesso);
            var existe = await sp.LerAsync(db => db.Categorias.AnyAsync(c => c.Id == 1));
            Assert.False(existe);
        }
    }
}
