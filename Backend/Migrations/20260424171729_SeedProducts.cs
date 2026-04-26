using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "Category", "Price", "Thumbnail", "Title" },
                values: new object[,]
                {
                    { 1, "Nature's Best", "fruits", 2.99m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Bananas", "Organic Bananas" },
                    { 2, "Dairy Farm", "dairies", 3.49m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Milk", "Whole Milk" },
                    { 3, "Farm Fresh", "meats", 8.99m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Chicken", "Chicken Breast" },
                    { 4, "Chef's Choice", "sauces", 1.99m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Tomato+Sauce", "Tomato Sauce" },
                    { 5, "SnackTime", "snacks", 4.50m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Chips", "Potato Chips" },
                    { 6, "Nature's Best", "vegetables", 2.50m, "https://via.placeholder.com/150/1E1E1E/FF5A00?text=Broccoli", "Fresh Broccoli" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
