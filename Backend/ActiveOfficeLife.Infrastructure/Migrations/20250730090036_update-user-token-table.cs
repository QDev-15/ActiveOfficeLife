using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateusertokentable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsResetPassword",
                table: "UserToken",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"), "Người dùng bình thường", "User" },
                    { new Guid("8928f10b-2598-4929-9085-d443b7e8bb4b"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("dfe041af-5e82-481e-9dc2-c99566e2da96"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("90f4ef06-32f1-45fc-a047-5edc3e3d809c"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "FullName", "PasswordHash", "PhoneNumber", "RefreshToken", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("7087c5d0-8dd7-441a-b956-5eddc39a58f8"), null, "activeoffice@gmail.com", null, "AQAAAAIAAYagAAAAEKn/KqklClhSD6cBRkUWfkHFnpXM9MoeSbeo+P1DHB+h2XjZufJVqjx/fLHzmHPiaw==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" },
                    { new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f"), null, "activeofficelife@gmail.com", null, "AQAAAAIAAYagAAAAEN7FaEQD/bDDI3MvRvKoVVhCgtU2mrHAcm7ViWfaeIML5NN4Qo/AgGSaXMWeRpiQAA==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"), new Guid("7087c5d0-8dd7-441a-b956-5eddc39a58f8") },
                    { new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"), new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f") },
                    { new Guid("dfe041af-5e82-481e-9dc2-c99566e2da96"), new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8928f10b-2598-4929-9085-d443b7e8bb4b"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("90f4ef06-32f1-45fc-a047-5edc3e3d809c"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"), new Guid("7087c5d0-8dd7-441a-b956-5eddc39a58f8") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"), new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("dfe041af-5e82-481e-9dc2-c99566e2da96"), new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3f52e74f-6c7b-4b09-b614-20d470bc7821"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("dfe041af-5e82-481e-9dc2-c99566e2da96"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7087c5d0-8dd7-441a-b956-5eddc39a58f8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("acf87f15-c3b3-4db9-810e-df01600fa54f"));

            migrationBuilder.DropColumn(
                name: "IsResetPassword",
                table: "UserToken");

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
    }
}
