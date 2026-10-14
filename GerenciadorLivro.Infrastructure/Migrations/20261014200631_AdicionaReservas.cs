using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorLivro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LivroId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataNotificacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEncerramento = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Livros_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_LivroId_Notificada",
                table: "Reservas",
                column: "LivroId",
                unique: true,
                filter: "[Status] = 'Notificada'");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_LivroId_Status_DataReserva",
                table: "Reservas",
                columns: new[] { "LivroId", "Status", "DataReserva" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_LivroId_UsuarioId_Pendente",
                table: "Reservas",
                columns: new[] { "LivroId", "UsuarioId" },
                unique: true,
                filter: "[Status] IN ('Ativa', 'Notificada')");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");
        }
    }
}
