using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixCuisineTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuisineTypes_Users_UserId",
                table: "CuisineTypes");

            migrationBuilder.DropIndex(
                name: "IX_CuisineTypes_UserId",
                table: "CuisineTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CuisineTypes");
        }

        /// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.AddColumn<int>(
        //        name: "UserId",
        //        table: "CuisineTypes",
        //        type: "INTEGER",
        //        nullable: false,
        //        defaultValue: 0);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_CuisineTypes_UserId",
        //        table: "CuisineTypes",
        //        column: "UserId");

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_CuisineTypes_Users_UserId",
        //        table: "CuisineTypes",
        //        column: "UserId",
        //        principalTable: "Users",
        //        principalColumn: "Id",
        //        onDelete: ReferentialAction.Cascade);
        //}
    }
}
