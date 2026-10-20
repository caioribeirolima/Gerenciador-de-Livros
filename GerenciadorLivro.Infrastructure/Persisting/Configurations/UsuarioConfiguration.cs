using GerenciadorLivro.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorLivro.Infrastructure.Persisting.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.Property(u => u.Cpf).HasMaxLength(11);

            // Última linha de defesa contra duas requisições simultâneas cadastrando o mesmo CPF.
            // Coluna anulável: o SQL Server cria o índice com filtro "[Cpf] IS NOT NULL", então usuários
            // antigos (sem CPF) não colidem entre si.
            builder.HasIndex(u => u.Cpf).IsUnique();
        }
    }
}