using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDisks.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddDiskCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByIssuer",
                table: "Disks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBySubject",
                table: "Disks",
                type: "text",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Disks_CreatedByIdentity",
                table: "Disks",
                sql: "(\"CreatedByIssuer\" IS NULL) = (\"CreatedBySubject\" IS NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Disks_CreatedByIdentity",
                table: "Disks");

            migrationBuilder.DropColumn(
                name: "CreatedByIssuer",
                table: "Disks");

            migrationBuilder.DropColumn(
                name: "CreatedBySubject",
                table: "Disks");
        }
    }
}
