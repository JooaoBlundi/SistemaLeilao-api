using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLeilao_api.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemLeilaoTableAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagens",
                table: "leiloes");

            migrationBuilder.CreateTable(
                name: "ImagensLeilao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeilaoId = table.Column<long>(type: "bigint", nullable: false),
                    DadosImagem = table.Column<byte[]>(type: "longblob", nullable: false),
                    ContentType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomeArquivo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagensLeilao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagensLeilao_leiloes_LeilaoId",
                        column: x => x.LeilaoId,
                        principalTable: "leiloes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ImagensLeilao_LeilaoId",
                table: "ImagensLeilao",
                column: "LeilaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagensLeilao");

            migrationBuilder.AddColumn<string>(
                name: "Imagens",
                table: "leiloes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
