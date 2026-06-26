using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019eea75-912b-76b9-83d6-cc08d5195692", "019eea75-912c-7cec-b07b-b63294913376", false, false, "Admin", "ADMIN" },
                    { "019eea75-912c-7cec-b07b-b631ff7d1068", "019eea75-912c-7cec-b07b-b63503db46fc", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019eea14-20e2-713c-9f2a-395c82aa848c", 0, "019eea14-20e2-713c-9f2a-395d878b8b11", "admin@Survey-Basket.com", true, "SurveyBasket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN", "Admin@123", null, false, "9BD53895FF5646F88FC363B0B838A11E", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 2, "permissions", "polls:add", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 3, "permissions", "polls:update", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 4, "permissions", "polls:delete", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 5, "permissions", "questions:read", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 6, "permissions", "questions:add", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 7, "permissions", "questions:update", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 8, "permissions", "users:read", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 9, "permissions", "users:add", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 10, "permissions", "users:update", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 11, "permissions", "roles:read", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 12, "permissions", "roles:add", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 13, "permissions", "roles:update", "019eea75-912b-76b9-83d6-cc08d5195692" },
                    { 14, "permissions", "results:read", "019eea75-912b-76b9-83d6-cc08d5195692" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019eea75-912b-76b9-83d6-cc08d5195692", "019eea14-20e2-713c-9f2a-395c82aa848c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019eea75-912c-7cec-b07b-b631ff7d1068");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019eea75-912b-76b9-83d6-cc08d5195692", "019eea14-20e2-713c-9f2a-395c82aa848c" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019eea75-912b-76b9-83d6-cc08d5195692");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019eea14-20e2-713c-9f2a-395c82aa848c");
        }
    }
}
