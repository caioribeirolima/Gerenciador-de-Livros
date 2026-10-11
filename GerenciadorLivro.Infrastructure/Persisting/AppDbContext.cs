using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Infrastructure.Persisting
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Editora> Editoras { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Livro>()
                .HasOne(l => l.Categoria)
                .WithMany(c => c.Livros)
                .HasForeignKey(l => l.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Livro>()
                .HasOne(l => l.Editora)
                .WithMany(e => e.Livros)
                .HasForeignKey(l => l.EditoraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Livro>()
                .HasMany(l => l.Autores)
                .WithMany(a => a.Livros);

            modelBuilder.Entity<Livro>()
                .HasQueryFilter(l => !l.Excluido);


            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.Livro)
                .WithMany(l => l.Emprestimos)
                .HasForeignKey(e => e.LivroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.Usuario)
                .WithMany(u => u.Emprestimos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}