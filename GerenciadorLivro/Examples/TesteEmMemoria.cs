using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Infrastructure.Persisting;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.API.Examples
{

    public static class TesteEmMemoria
    {
        public static async Task RodarAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TesteLivros")
                .Options;

            using var context = new AppDbContext(options);

            context.Livros.Add(new Livro
            {
                Titulo = "Livro Teste 1",
                AnoPublicacao = 2020,
                Disponivel = true,
                CategoriaId = 1,
                EditoraId = 1
            });

            context.Livros.Add(new Livro
            {
                Titulo = "Livro Teste 2",
                AnoPublicacao = 2021,
                Disponivel = true,
                CategoriaId = 1,
                EditoraId = 1
            });

            await context.SaveChangesAsync();

            var todosLivros = await context.Livros.ToListAsync();

            foreach (var livro in todosLivros)
                Console.WriteLine($"{livro.Id} - {livro.Titulo} ({livro.AnoPublicacao})");

        }
    }
}
