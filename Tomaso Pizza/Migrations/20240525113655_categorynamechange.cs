using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tomaso_Pizza.Migrations
{
    /// <inheritdoc />
    public partial class categorynamechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Category",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Category",
                newName: "CategoryId");
        }
    }
}
