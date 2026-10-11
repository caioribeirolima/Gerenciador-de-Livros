using GerenciadorLivro.Application.Commands.CriarLivro;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Interfaces;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Exceptions;
using GerenciadorLivro.Infrastructure.Persisting;
using GerenciadorLivro.Infrastructure.Services;
using GerenciadorLivro.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

            builder.Services.AddScoped<IEmprestimoService, EmprestimoService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IEditoraService, EditoraService>();

            builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(CriarLivroCommand).Assembly));

            builder.Services.Configure<EmprestimoSettings>(
                builder.Configuration.GetSection("EmprestimoSettings"));
            builder.Services.Configure<UploadSettings>(
                builder.Configuration.GetSection("UploadSettings"));

            var app = builder.Build();

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var statusCode = exceptionFeature?.Error switch
                    {
                        LivroInvalidoException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/problem+json";

                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Title = statusCode == StatusCodes.Status400BadRequest
                            ? "Requisição inválida"
                            : "Erro interno no servidor",
                        Detail = exceptionFeature?.Error.Message,
                        Status = statusCode
                    });
                });
            });

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