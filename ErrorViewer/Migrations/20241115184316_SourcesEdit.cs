using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErrorViewer.Migrations
{
    /// <inheritdoc />
    public partial class SourcesEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "connectionString",
                table: "Sources",
                newName: "ConnectionString");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Sources",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConnectionString",
                table: "Sources",
                newName: "connectionString");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Sources",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
