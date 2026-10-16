using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreteriaPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CategoriaPadreId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_categorias_categorias_CategoriaPadreId",
                        column: x => x.CategoriaPadreId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NombreCompleto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CodigoBarras = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Variante = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StockActual = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StockMinimo = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    UnidadMedida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PermitirRecalculo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productos_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tipos_cambio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorCompra = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ValorVenta = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaVigencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistradoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_cambio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tipos_cambio_usuarios_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroVenta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VendedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonedaPago = table.Column<int>(type: "integer", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Cambio = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TipoCambioAplicado = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ventas_usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "formulas_precios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefinicionJson = table.Column<string>(type: "jsonb", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: true),
                    EsGlobal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formulas_precios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_formulas_precios_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_formulas_precios_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "precios_productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoPrecio = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Moneda = table.Column<int>(type: "integer", nullable: false),
                    FechaVigencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precios_productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_precios_productos_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalles_ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductoSKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonedaPrecio = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalles_ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detalles_ventas_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_detalles_ventas_ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_inventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMovimiento = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MonedaPrecio = table.Column<int>(type: "integer", nullable: true),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistradoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos_inventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movimientos_inventario_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_inventario_usuarios_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_inventario_ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categorias_CategoriaPadreId",
                table: "categorias",
                column: "CategoriaPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_ventas_ProductoId",
                table: "detalles_ventas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_ventas_VentaId",
                table: "detalles_ventas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_formulas_precios_CategoriaId",
                table: "formulas_precios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_formulas_precios_EsGlobal",
                table: "formulas_precios",
                column: "EsGlobal");

            migrationBuilder.CreateIndex(
                name: "IX_formulas_precios_ProductoId",
                table: "formulas_precios",
                column: "ProductoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_inventario_CreadoEn",
                table: "movimientos_inventario",
                column: "CreadoEn");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_inventario_ProductoId",
                table: "movimientos_inventario",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_inventario_RegistradoPorId",
                table: "movimientos_inventario",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_inventario_VentaId",
                table: "movimientos_inventario",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_precios_productos_ProductoId_TipoPrecio_Activo",
                table: "precios_productos",
                columns: new[] { "ProductoId", "TipoPrecio", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_productos_CategoriaId",
                table: "productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_productos_CodigoBarras",
                table: "productos",
                column: "CodigoBarras");

            migrationBuilder.CreateIndex(
                name: "IX_productos_SKU",
                table: "productos",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipos_cambio_FechaVigencia",
                table: "tipos_cambio",
                column: "FechaVigencia");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_cambio_RegistradoPorId",
                table: "tipos_cambio",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NombreUsuario",
                table: "usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ventas_FechaVenta",
                table: "ventas",
                column: "FechaVenta");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_NumeroVenta",
                table: "ventas",
                column: "NumeroVenta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ventas_VendedorId",
                table: "ventas",
                column: "VendedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalles_ventas");

            migrationBuilder.DropTable(
                name: "formulas_precios");

            migrationBuilder.DropTable(
                name: "movimientos_inventario");

            migrationBuilder.DropTable(
                name: "precios_productos");

            migrationBuilder.DropTable(
                name: "tipos_cambio");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
