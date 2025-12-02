using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIABank.DA.Migrations
{
    /// <inheritdoc />
    public partial class FixTransferenciasUnificadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProveedoresServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinLongitudContrato = table.Column<int>(type: "int", nullable: false),
                    MaxLongitudContrato = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedoresServicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuentaOrigenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuentaDestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TerceroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsuarioEjecutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transferencias_Beneficiarios_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "Beneficiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Cuentas_CuentaDestinoId",
                        column: x => x.CuentaDestinoId,
                        principalTable: "Cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Cuentas_CuentaOrigenId",
                        column: x => x.CuentaOrigenId,
                        principalTable: "Cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagosServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroReferencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProveedorServicioId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NumeroContrato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CuentaOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosServicios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosServicios_ProveedoresServicios_ProveedorServicioId",
                        column: x => x.ProveedorServicioId,
                        principalTable: "ProveedoresServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesTransferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferenciaId = table.Column<int>(type: "int", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ejecutada = table.Column<bool>(type: "bit", nullable: false),
                    Cancelada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesTransferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramacionesTransferencias_Transferencias_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "Transferencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosServicios_ClienteId",
                table: "PagosServicios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosServicios_ProveedorServicioId",
                table: "PagosServicios",
                column: "ProveedorServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesTransferencias_TransferenciaId",
                table: "ProgramacionesTransferencias",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CuentaDestinoId",
                table: "Transferencias",
                column: "CuentaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CuentaOrigenId",
                table: "Transferencias",
                column: "CuentaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_IdempotencyKey",
                table: "Transferencias",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_Referencia",
                table: "Transferencias",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_TerceroId",
                table: "Transferencias",
                column: "TerceroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosServicios");

            migrationBuilder.DropTable(
                name: "ProgramacionesTransferencias");

            migrationBuilder.DropTable(
                name: "ProveedoresServicios");

            migrationBuilder.DropTable(
                name: "Transferencias");
        }
    }
}
