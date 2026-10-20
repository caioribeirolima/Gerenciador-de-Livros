using FluentValidation;
using GerenciadorLivro.Application.Commands.CriarLivro;
using GerenciadorLivro.Application.Common.Behaviors;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using GerenciadorLivro.Infrastructure.Persisting;
using GerenciadorLivro.Infrastructure.Persisting.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CriarLivroCommand).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssembly(typeof(CriarLivroCommand).Assembly);

            builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            builder.Services.AddScoped<ILivroRepository, LivroRepository>();
            builder.Services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
            builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            builder.Services.AddScoped<IEditoraRepository, EditoraRepository>();
            builder.Services.AddScoped<IAutorRepository, AutorRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddProblemDetails();

            builder.Services.Configure<EmprestimoSettings>(
                builder.Configuration.GetSection("EmprestimoSettings"));
            builder.Services.Configure<UploadSettings>(
                builder.Configuration.GetSection("UploadSettings"));
            builder.Services.Configure<PerformanceSettings>(
                builder.Configuration.GetSection("PerformanceSettings"));

            var app = builder.Build();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}