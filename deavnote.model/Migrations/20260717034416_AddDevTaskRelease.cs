using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace deavnote.model.Migrations
{
    /// <inheritdoc />
    public partial class AddDevTaskRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Release",
                table: "DevTasks",
                type: "TEXT",
                maxLength: 32,
                nullable: true,
                defaultValue: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Release",
                table: "DevTasks");
        }
    }
}
