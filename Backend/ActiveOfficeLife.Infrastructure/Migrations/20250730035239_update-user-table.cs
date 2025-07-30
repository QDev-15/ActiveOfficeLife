using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6d6ecd29-6ace-4ed2-8a38-97fa22221924"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("9d528a17-ade9-4c4f-8556-c67fce80c61e"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"), new Guid("2659ca4a-b0ae-490c-9b09-fe8bcad44a1f") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("089383b1-65dc-401b-b671-88ec3f08086c"), new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"), new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("089383b1-65dc-401b-b671-88ec3f08086c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2659ca4a-b0ae-490c-9b09-fe8bcad44a1f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd"));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"), "Người dùng bình thường", "User" },
                    { new Guid("81719807-722f-4305-8419-6d840a6473f7"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("be8e8976-ecac-4585-a918-8618da5ce393"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("126244e8-dccf-4c0b-a11d-9a72168bbe94"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "FullName", "PasswordHash", "PhoneNumber", "RefreshToken", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4"), null, "activeofficelife@gmail.com", null, "AQAAAAIAAYagAAAAEOsfyF9SP1bCUzhMxGZa+P793XNERCHLs9UBJwkZgxqZLSJtYfPEg59NTzQjJBERzQ==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("3b59f774-fc72-4d92-9a3a-9a31d157ec4b"), null, "activeoffice@gmail.com", null, "AQAAAAIAAYagAAAAEFVFThictOVGW6REEYz4oXipcrjB5YJ/Y+Ocit6EDpxb6FOmx80qSANmNqndSdJkwQ==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"), new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4") },
                    { new Guid("be8e8976-ecac-4585-a918-8618da5ce393"), new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4") },
                    { new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"), new Guid("3b59f774-fc72-4d92-9a3a-9a31d157ec4b") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("81719807-722f-4305-8419-6d840a6473f7"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("126244e8-dccf-4c0b-a11d-9a72168bbe94"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"), new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("be8e8976-ecac-4585-a918-8618da5ce393"), new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"), new Guid("3b59f774-fc72-4d92-9a3a-9a31d157ec4b") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("52c38ce9-18e0-446f-aed4-7a7e5edc4254"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("be8e8976-ecac-4585-a918-8618da5ce393"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1087f50b-cda5-43ed-858b-345a1e76bdc4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3b59f774-fc72-4d92-9a3a-9a31d157ec4b"));

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("089383b1-65dc-401b-b671-88ec3f08086c"), "Quản trị viên hệ thống", "Admin" },
                    { new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"), "Người dùng bình thường", "User" },
                    { new Guid("6d6ecd29-6ace-4ed2-8a38-97fa22221924"), "Người đọc chỉ comment", "Comment" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("9d528a17-ade9-4c4f-8556-c67fce80c61e"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "PasswordHash", "RefreshToken", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("2659ca4a-b0ae-490c-9b09-fe8bcad44a1f"), null, "activeoffice@gmail.com", "AQAAAAIAAYagAAAAEEsfUd/AKIGqz4rFPbzRHl6RAexwcV76U1avBgv4KjODBctPqY4a+RXrD04RBVqIlA==", null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" },
                    { new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd"), null, "activeofficelife@gmail.com", "AQAAAAIAAYagAAAAEKUK2wZQg20z1C0qJSHIx5qfpH6DepPWIGopJF83Wny627Pju1+PFQckDGa+j1CI3A==", null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"), new Guid("2659ca4a-b0ae-490c-9b09-fe8bcad44a1f") },
                    { new Guid("089383b1-65dc-401b-b671-88ec3f08086c"), new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd") },
                    { new Guid("6cf393c7-7857-404b-8d4e-b128903abafb"), new Guid("5d871d90-e875-4162-94ab-9dfa0d4686dd") }
                });
        }
    }
}
