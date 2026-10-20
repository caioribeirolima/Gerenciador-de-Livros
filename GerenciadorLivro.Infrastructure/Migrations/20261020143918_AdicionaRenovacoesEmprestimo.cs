using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorLivro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaRenovacoesEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Renovacoes",
                table: "Emprestimos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Renovacoes",
                table: "Emprestimos");
        }
    }
}
