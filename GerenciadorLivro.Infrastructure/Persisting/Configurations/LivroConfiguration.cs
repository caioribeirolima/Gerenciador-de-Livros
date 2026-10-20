using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorLivro.Infrastructure.Persisting.Configurations
{
    public class LivroConfiguration : IEntityTypeConfiguration<Livro>
    {
        public void Configure(EntityTypeBuilder<Livro> builder)
        {
            builder.HasOne(l => l.Categoria)
                .WithMany(c => c.Livros)
                .HasForeignKey(l => l.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Editora)
                .WithMany(e => e.Livros)
                .HasForeignKey(l => l.EditoraId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(l => l.Autores)
                .WithMany(a => a.Livros);

            builder.HasQueryFilter(l => !l.Excluido);
            builder.Property(l => l.Titulo).IsRequired().HasMaxLength(200);
        }
    }
}