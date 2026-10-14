using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorLivro.Infrastructure.Persisting.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.DataReserva)
                .IsRequired();

            // Enum gravado como texto: legível no banco e permite índices filtrados por nome.
            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(r => r.Livro)
                .WithMany(l => l.Reservas)
                .HasForeignKey(r => r.LivroId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fila do livro: busca a primeira Ativa por DataReserva.
            builder.HasIndex(r => new { r.LivroId, r.Status, r.DataReserva });

            // Rede de segurança contra corrida: um usuário não pode ter duas reservas
            // "vivas" (Ativa ou Notificada) para o mesmo livro.
            builder.HasIndex(r => new { r.LivroId, r.UsuarioId })
                .IsUnique()
                .HasFilter("[Status] IN ('Ativa', 'Notificada')")
                .HasDatabaseName("IX_Reservas_LivroId_UsuarioId_Pendente");

            // No máximo uma reserva Notificada por livro.
            builder.HasIndex(r => r.LivroId)
                .IsUnique()
                .HasFilter("[Status] = 'Notificada'")
                .HasDatabaseName("IX_Reservas_LivroId_Notificada");
        }
    }
}
