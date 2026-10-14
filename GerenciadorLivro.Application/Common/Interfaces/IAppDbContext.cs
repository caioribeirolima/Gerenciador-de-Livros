using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Livro> Livros { get; }
        DbSet<Autor> Autores { get; }
        DbSet<Categoria> Categorias { get; }
        DbSet<Emprestimo> Emprestimos { get; }
        DbSet<Usuario> Usuarios { get; }
        DbSet<Editora> Editoras { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
