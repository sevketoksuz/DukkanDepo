using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DukkanDepo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KislikUrunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Kod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Cins = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Iskonto = table.Column<double>(type: "REAL", nullable: true),
                    Adet = table.Column<int>(type: "INTEGER", nullable: true),
                    Satis = table.Column<double>(type: "REAL", nullable: true),
                    Tutar = table.Column<double>(type: "REAL", nullable: true),
                    IskontoluTutar = table.Column<double>(type: "REAL", nullable: true),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KislikUrunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YazlikUrunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Kod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Cins = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Iskonto = table.Column<double>(type: "REAL", nullable: true),
                    Adet = table.Column<int>(type: "INTEGER", nullable: true),
                    Satis = table.Column<double>(type: "REAL", nullable: true),
                    Tutar = table.Column<double>(type: "REAL", nullable: true),
                    IskontoluTutar = table.Column<double>(type: "REAL", nullable: true),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YazlikUrunler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KislikUrunler");

            migrationBuilder.DropTable(
                name: "YazlikUrunler");
        }
    }
}
