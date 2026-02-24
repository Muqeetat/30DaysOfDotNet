using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDVerificationAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVerificationReqColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalIdSubmitted",
                table: "VerificationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "VerificationRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalIdSubmitted",
                table: "VerificationRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "VerificationRequests");
        }
    }
}
