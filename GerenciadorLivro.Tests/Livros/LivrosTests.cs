using GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro;
using GerenciadorLivro.Application.Commands.CriarAutor;
using GerenciadorLivro.Application.Commands.CriarEditora;
using GerenciadorLivro.Application.Commands.CriarLivro;
using GerenciadorLivro.Application.Commands.EditarLivro;
using GerenciadorLivro.Application.Commands.ExcluirEditora;
using GerenciadorLivro.Application.Commands.ExcluirLivro;
using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GerenciadorLivro.Tests.Reservas.BancoDeTeste;

namespace GerenciadorLivro.Tests.Livros
{
    public class LivrosTests
    {
        private static Autor Autor(int id, string nome) => new Autor { Id = id, Nome = nome };
        private static Editora Editora(int id, string nome) => new Editora { Id = id, Nome = nome };

        private static async Task<TResponse> EnviarAsync<TResponse>(ServiceProvider sp, IRequest<TResponse> request)
        {
            using var scope = sp.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        // ---------- CriarLivro ----------

        [Fact]
        public async Task CriarLivro_ComAutoresVinculados_PersisteAssociacoes()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Autores.AddRange(Autor(1, "Autor 1"), Autor(2, "Autor 2"));
            });

            var result = await EnviarAsync(sp, new CriarLivroCommand
            {
                Titulo = "Livro com autores",
                AnoPublicacao = 2020,
                CategoriaId = 1,
                EditoraId = 1,
                AutorIds = new List<int> { 1, 2 }
            });

            Assert.True(result.Sucesso);
            var autoresIds = await sp.LerAsync(db => db.Livros
                .Where(l => l.Id == result.Valor)
                .SelectMany(l => l.Autores.Select(a => a.Id))
                .OrderBy(id => id)
                .ToListAsync());
            Assert.Equal(new[] { 1, 2 }, autoresIds);
        }

        [Fact]
        public async Task CriarLivro_ComAnoFuturo_Falha()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarLivroCommand
            {
                Titulo = "Do futuro",
                AnoPublicacao = DateTime.Now.Year + 1,
                CategoriaId = 1,
                EditoraId = 1
            });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarLivroCommand.AnoPublicacao)));
            var totalLivros = await sp.LerAsync(db => db.Livros.CountAsync());
            Assert.Equal(0, totalLivros);
        }

        [Fact]
        public async Task CriarLivro_TituloVazio_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarLivroCommand
            {
                Titulo = "",
                AnoPublicacao = 2020,
                CategoriaId = 1,
                EditoraId = 1
            });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarLivroCommand.Titulo)));
            var totalLivros = await sp.LerAsync(db => db.Livros.CountAsync());
            Assert.Equal(0, totalLivros);
        }

        [Fact]
        public async Task CriarLivro_TituloVazioEAnoFuturo_ReportaOsDoisErros()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarLivroCommand
            {
                Titulo = "",
                AnoPublicacao = DateTime.Now.Year + 1,
                CategoriaId = 1,
                EditoraId = 1
            });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarLivroCommand.Titulo)));
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarLivroCommand.AnoPublicacao)));
        }

        // ---------- EditarLivro ----------

        [Fact]
        public async Task EditarLivro_ComSucesso_AtualizaCampos()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Livros.Add(Livro(1, true)));

            var result = await EnviarAsync(sp, new EditarLivroCommand
            {
                Id = 1,
                Titulo = "Novo título",
                AnoPublicacao = 2021,
                CategoriaId = 2,
                EditoraId = 3
            });

            Assert.True(result.Sucesso);
            var livro = await sp.LerAsync(db => db.Livros.SingleAsync(l => l.Id == 1));
            Assert.Equal("Novo título", livro.Titulo);
            Assert.Equal(2021, livro.AnoPublicacao);
            Assert.Equal(2, livro.CategoriaId);
            Assert.Equal(3, livro.EditoraId);
        }

        [Fact]
        public async Task EditarLivro_LivroExcluido_NaoEncontradoSemAlterarRegistro()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                var livro = Livro(1, true);
                livro.Excluido = true;
                db.Livros.Add(livro);
            });

            var result = await EnviarAsync(sp, new EditarLivroCommand
            {
                Id = 1,
                Titulo = "Novo título",
                AnoPublicacao = 2021,
                CategoriaId = 2,
                EditoraId = 3
            });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
            var livro = await sp.LerAsync(db => db.Livros.IgnoreQueryFilters().SingleAsync(l => l.Id == 1));
            Assert.Equal("Livro 1", livro.Titulo);
        }

        [Fact]
        public async Task EditarLivro_TituloVazio_RetornaValidacaoSemAlterarRegistro()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Livros.Add(Livro(1, true)));

            var result = await EnviarAsync(sp, new EditarLivroCommand
            {
                Id = 1,
                Titulo = "",
                AnoPublicacao = 2021,
                CategoriaId = 2,
                EditoraId = 3
            });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(EditarLivroCommand.Titulo)));
            var livro = await sp.LerAsync(db => db.Livros.SingleAsync(l => l.Id == 1));
            Assert.Equal("Livro 1", livro.Titulo);
        }

        // ---------- ExcluirLivro ----------

        [Fact]
        public async Task ExcluirLivro_MarcaExcluidoSemRemoverRegistro()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Livros.Add(Livro(1, true)));

            var result = await EnviarAsync(sp, new ExcluirLivroCommand { Id = 1 });

            Assert.True(result.Sucesso);
            var visivel = await sp.LerAsync(db => db.Livros.AnyAsync(l => l.Id == 1));
            Assert.False(visivel);
            var persistido = await sp.LerAsync(db => db.Livros
                .IgnoreQueryFilters()
                .SingleAsync(l => l.Id == 1));
            Assert.True(persistido.Excluido);
        }

        [Fact]
        public async Task ExcluirLivro_LivroInexistente_NaoEncontrado()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new ExcluirLivroCommand { Id = 99 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
        }

        // ---------- AdicionarAutorAoLivro ----------

        [Fact]
        public async Task AdicionarAutorAoLivro_ComSucesso()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Livros.Add(Livro(1, true));
                db.Autores.Add(Autor(1, "Autor 1"));
            });

            var result = await EnviarAsync(sp, new AdicionarAutorAoLivroCommand { LivroId = 1, AutorId = 1 });

            Assert.True(result.Sucesso);
            var autoresIds = await sp.LerAsync(db => db.Livros
                .Where(l => l.Id == 1)
                .SelectMany(l => l.Autores.Select(a => a.Id))
                .ToListAsync());
            Assert.Equal(new[] { 1 }, autoresIds);
        }

        [Fact]
        public async Task AdicionarAutorAoLivro_AutorJaAssociado_Conflito()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                var livro = Livro(1, true);
                var autor = Autor(1, "Autor 1");
                livro.Autores.Add(autor);
                db.Livros.Add(livro);
            });

            var result = await EnviarAsync(sp, new AdicionarAutorAoLivroCommand { LivroId = 1, AutorId = 1 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Conflito, result.Tipo);
        }

        [Fact]
        public async Task AdicionarAutorAoLivro_AutorInexistente_NaoEncontrado()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Livros.Add(Livro(1, true)));

            var result = await EnviarAsync(sp, new AdicionarAutorAoLivroCommand { LivroId = 1, AutorId = 99 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
            Assert.Contains("Autor de id 99", result.Erro);
        }

        [Fact]
        public async Task AdicionarAutorAoLivro_LivroInexistente_NaoEncontrado()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Autores.Add(Autor(1, "Autor 1")));

            var result = await EnviarAsync(sp, new AdicionarAutorAoLivroCommand { LivroId = 99, AutorId = 1 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
            Assert.Contains("Livro de id 99", result.Erro);
        }

        // ---------- CriarAutor ----------

        [Fact]
        public async Task CriarAutor_NomeDuplicadoCaseDiferente_Falha()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Autores.Add(Autor(1, "Machado de Assis")));

            var result = await EnviarAsync(sp, new CriarAutorCommand { Nome = "machado DE assis" });

            Assert.False(result.Sucesso);
            Assert.Contains("já registrado", result.Erro);
            var total = await sp.LerAsync(db => db.Autores.CountAsync());
            Assert.Equal(1, total);
        }

        [Fact]
        public async Task CriarAutor_NomeMaiorQueOLimite_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarAutorCommand { Nome = new string('a', 151) });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarAutorCommand.Nome)));
            var total = await sp.LerAsync(db => db.Autores.CountAsync());
            Assert.Equal(0, total);
        }

        [Fact]
        public async Task CriarAutor_NomeNoLimite_Sucesso()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarAutorCommand { Nome = new string('a', 150) });

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task CriarAutor_NomeVazio_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarAutorCommand { Nome = "   " });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarAutorCommand.Nome)));
            var total = await sp.LerAsync(db => db.Autores.CountAsync());
            Assert.Equal(0, total);
        }

        [Fact]
        public async Task CriarEditora_NomeVazio_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarEditoraCommand { Nome = "   " });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            Assert.True(result.ErrosValidacao!.ContainsKey(nameof(CriarEditoraCommand.Nome)));
            var total = await sp.LerAsync(db => db.Editoras.CountAsync());
            Assert.Equal(0, total);
        }

        [Fact]
        public async Task CriarEditora_NomeMaiorQue100_RetornaValidacaoSemGravar()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new CriarEditoraCommand { Nome = new string('a', 101) });

            Assert.Equal(TipoErro.Validacao, result.Tipo);
            var total = await sp.LerAsync(db => db.Editoras.CountAsync());
            Assert.Equal(0, total);
        }

        // ---------- ExcluirEditora ----------

        [Fact]
        public async Task ExcluirEditora_ComLivroExcluidoVinculado_Conflito()
        {
            using var sp = Criar();
            await sp.SemearAsync(db =>
            {
                db.Editoras.Add(Editora(1, "Editora 1"));
                var livro = Livro(1, true); // EditoraId = 1
                livro.Excluido = true;
                db.Livros.Add(livro);
            });

            var result = await EnviarAsync(sp, new ExcluirEditoraCommand { Id = 1 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Conflito, result.Tipo);
            Assert.Contains("livros vinculados", result.Erro);
        }

        [Fact]
        public async Task ExcluirEditora_EditoraInexistente_NaoEncontrado()
        {
            using var sp = Criar();

            var result = await EnviarAsync(sp, new ExcluirEditoraCommand { Id = 99 });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.NaoEncontrado, result.Tipo);
        }

        [Fact]
        public async Task CriarEditora_NomeDuplicadoCaseDiferente_Conflito()
        {
            using var sp = Criar();
            await sp.SemearAsync(db => db.Editoras.Add(Editora(1, "Companhia das Letras")));

            var result = await EnviarAsync(sp, new CriarEditoraCommand { Nome = "companhia DAS letras" });

            Assert.False(result.Sucesso);
            Assert.Equal(TipoErro.Conflito, result.Tipo);
            var total = await sp.LerAsync(db => db.Editoras.CountAsync());
            Assert.Equal(1, total);
        }
    }
}
