using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErrorViewer.Migrations
{
    /// <inheritdoc />
    public partial class EditSource2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cahceTime",
                table: "Sources",
                newName: "cacheTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cacheTime",
                table: "Sources",
                newName: "cahceTime");
        }
    }
}
