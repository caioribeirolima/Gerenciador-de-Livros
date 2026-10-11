using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Livro> Livros { get; }
        DbSet<Autor> Autores { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
