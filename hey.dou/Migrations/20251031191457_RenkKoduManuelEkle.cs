using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hey.dou.Migrations
{
    /// <inheritdoc />
    public partial class RenkKoduManuelEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EF'nin CreateTable komutlarını sildik.
            // Sadece 'RenkKodu' kolonunu eklemesi için manuel SQL komutu yazıyoruz.
            migrationBuilder.Sql("ALTER TABLE AkademikTakvim ADD RenkKodu nvarchar(max) NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri almak istersek 'RenkKodu' kolonunu silmesi için komut.
            migrationBuilder.Sql("ALTER TABLE AkademikTakvim DROP COLUMN RenkKodu");
        }
    }
}