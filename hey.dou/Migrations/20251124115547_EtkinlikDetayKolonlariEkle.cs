using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hey.dou.Migrations
{
    /// <inheritdoc />
    public partial class EtkinlikDetayKolonlariEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KisaAciklama",
                table: "AnketSecenegis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "AnketSecenegis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KulupAdi",
                table: "AnketSecenegis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KulupBaskaniAdi",
                table: "AnketSecenegis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TarihSaat",
                table: "AnketSecenegis",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KisaAciklama",
                table: "AnketSecenegis");

            migrationBuilder.DropColumn(
                name: "Konum",
                table: "AnketSecenegis");

            migrationBuilder.DropColumn(
                name: "KulupAdi",
                table: "AnketSecenegis");

            migrationBuilder.DropColumn(
                name: "KulupBaskaniAdi",
                table: "AnketSecenegis");

            migrationBuilder.DropColumn(
                name: "TarihSaat",
                table: "AnketSecenegis");
        }
    }
}
