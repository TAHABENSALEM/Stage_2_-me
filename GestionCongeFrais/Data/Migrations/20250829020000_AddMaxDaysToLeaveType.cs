using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionCongeFrais.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxDaysToLeaveType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDays",
                table: "LeaveTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDays",
                table: "LeaveTypes");
        }
    }
}
