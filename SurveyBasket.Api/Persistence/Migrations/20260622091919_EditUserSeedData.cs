using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019eea14-20e2-713c-9f2a-395c82aa848c",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEP7bd0t9M0qOCsSmg6nspHIHx9rdLDt3/FlxL32oJguf3zkwbeC+OXKOT+j3vmP0Gg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019eea14-20e2-713c-9f2a-395c82aa848c",
                column: "PasswordHash",
                value: "Admin@123");
        }
    }
}
