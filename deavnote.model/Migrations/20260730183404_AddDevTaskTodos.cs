using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace deavnote.model.Migrations
{
    /// <inheritdoc />
    public partial class AddDevTaskTodos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Todos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TaskId",
                table: "Todos",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_DevTasks_TaskId",
                table: "Todos",
                column: "TaskId",
                principalTable: "DevTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_DevTasks_TaskId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_TaskId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Todos");
        }
    }
}
