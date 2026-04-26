using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Bananas");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Milk");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Chicken");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Tomato+Sauce");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Chips");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "Thumbnail",
                value: "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Broccoli");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Bananas");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Milk");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Chicken");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Tomato+Sauce");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Chips");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "Thumbnail",
                value: "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Broccoli");
        }
    }
}
