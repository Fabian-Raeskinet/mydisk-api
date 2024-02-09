using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDisks.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RenamePseudonymProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pseudonyme",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "Pseudonym",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<double>(type: "float", nullable: false),
                    PublishedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Disks_DiskId",
                        column: x => x.DiskId,
                        principalTable: "Disks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_DiskId",
                table: "Review",
                column: "DiskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropColumn(
                name: "Pseudonym",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "Pseudonyme",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
