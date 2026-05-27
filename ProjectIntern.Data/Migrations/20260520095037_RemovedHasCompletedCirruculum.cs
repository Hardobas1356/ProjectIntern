using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectIntern.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedHasCompletedCirruculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCompletedCurriculum",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCompletedCurriculum",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
