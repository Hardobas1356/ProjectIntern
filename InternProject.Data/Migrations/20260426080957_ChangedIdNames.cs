using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectIntern.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIdNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkDayAssigmentId",
                table: "WorkDayAssignments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TopicID",
                table: "Topics",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InternshipSpecialityID",
                table: "InternshipSpecialities",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WorkDayAssignments",
                newName: "WorkDayAssigmentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Topics",
                newName: "TopicID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InternshipSpecialities",
                newName: "InternshipSpecialityID");
        }
    }
}
