using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hey.dou.Migrations
{
    /// <inheritdoc />
    public partial class CreatorKullaniciKolonu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // === BU KISIM SADECE YENİ KOLONU EKLER ===
            // 1. Amacımız olan CreatorKullaniciId kolonunu AnketSecenegis tablosuna ekliyoruz.
            migrationBuilder.AddColumn<string>(
                name: "CreatorKullaniciId",
                table: "AnketSecenegis",
                type: "nvarchar(max)",
                nullable: true);

            // Tüm CreateTable ve CreateIndex komutları SİLİNDİ.
            // Bu, veritabanının mevcut yapısını korur.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri alma işleminde sadece bu kolonu sileriz.
            migrationBuilder.DropColumn(
                name: "CreatorKullaniciId",
                table: "AnketSecenegis");

            // Tüm DropTable komutları SİLİNDİ.
        }
    }
}