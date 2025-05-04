using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLeilao_api.Migrations
{
    /// <inheritdoc />
    public partial class AddManterConectadoAteToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ManterConectadoAte",
                table: "usuarios",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManterConectadoAte",
                table: "usuarios");
        }
    }
}
