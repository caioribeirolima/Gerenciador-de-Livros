using GerenciadorLivro.Application.Commands.ReservarLivro;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using GerenciadorLivro.Infrastructure.Persisting;
using GerenciadorLivro.Infrastructure.Persisting.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorLivro.Tests.Reservas
{
    public static class BancoDeTeste
    {
        public static ServiceProvider Criar(Action<IServiceCollection>? extras = null)
        {
            var nomeBanco = Guid.NewGuid().ToString();
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(nomeBanco));
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<ILivroRepository, LivroRepository>();
            services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Configure<EmprestimoSettings>(s => s.DiasParaDevolucao = 7);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReservarLivroCommand).Assembly));

            extras?.Invoke(services);
            return services.BuildServiceProvider();
        }

        public static async Task SemearAsync(this ServiceProvider sp, Action<AppDbContext> semear)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            semear(db);
            await db.SaveChangesAsync();
        }

        public static async Task<T> LerAsync<T>(this ServiceProvider sp, Func<AppDbContext, Task<T>> ler)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await ler(db);
        }

        public static Livro Livro(int id, bool disponivel) => new Livro
        {
            Id = id,
            Titulo = $"Livro {id}",
            AnoPublicacao = 2020,
            Disponivel = disponivel,
            CategoriaId = 1,
            EditoraId = 1
        };

        public static Usuario Usuario(int id) => new Usuario($"Usuario {id}", $"u{id}@teste.com") { Id = id };

        public static Reserva Reserva(int id, int livroId, int usuarioId, DateTime data, StatusReserva status = StatusReserva.Ativa)
            => new Reserva(livroId, usuarioId, data) { Id = id, Status = status };

        public static Emprestimo EmprestimoAberto(int id, int livroId, int usuarioId) => new Emprestimo
        {
            Id = id,
            LivroId = livroId,
            UsuarioId = usuarioId,
            DataEmprestimo = DateTime.Now.AddDays(-3),
            DataDevolucaoPrevista = DateTime.Now.AddDays(4)
        };
    }
}
