using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIABank.DA.Migrations
{
    /// <inheritdoc />
    public partial class AddCuentasYBeneficiarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Moneda = table.Column<int>(type: "int", nullable: false),
                    NumeroCuenta = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Numero = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Moneda = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_ClienteId_Alias",
                table: "Beneficiarios",
                columns: new[] { "ClienteId", "Alias" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_Numero",
                table: "Cuentas",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "Cuentas");
        }
    }
}
