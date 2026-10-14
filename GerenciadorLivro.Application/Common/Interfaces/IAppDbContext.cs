using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Livro> Livros { get; }
        DbSet<Autor> Autores { get; }
        DbSet<Categoria> Categorias { get; }
        DbSet<Emprestimo> Emprestimos { get; set; }
        DbSet<Usuario> Usuarios { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
