using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErrorViewer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDataToSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cahceTime",
                table: "Sources",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "errorRow",
                table: "Sources",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cahceTime",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "errorRow",
                table: "Sources");
        }
    }
}
