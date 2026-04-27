using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "PantryItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "Calories",
                table: "PantryItems",
                newName: "UnitId");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PantryItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PantryItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PantryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "OrderProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitId",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_ProductId",
                table: "PantryItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PantryItems_UnitId",
                table: "PantryItems",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_UnitId",
                table: "OrderProducts",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Units_UnitId",
                table: "OrderProducts",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_Products_ProductId",
                table: "PantryItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_Units_UnitId",
                table: "PantryItems",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Units_UnitId",
                table: "Products",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Units_UnitId",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_Products_ProductId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_Units_UnitId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Units_UnitId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Products_UnitId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_PantryItems_ProductId",
                table: "PantryItems");

            migrationBuilder.DropIndex(
                name: "IX_PantryItems_UnitId",
                table: "PantryItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_UnitId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PantryItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "PantryItems",
                newName: "Calories");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PantryItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "PantryItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "PantryItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "OrderProducts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
