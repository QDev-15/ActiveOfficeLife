using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("9ac0d197-5234-4605-b66b-83528c9cf906"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("33087857-4eca-437a-a2b4-411d79916fe8"), new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("679a3324-071c-4cc4-967f-78e06944566a"), new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("33087857-4eca-437a-a2b4-411d79916fe8"), new Guid("df5b143e-4c88-4b8b-9019-c205a94c78f4") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33087857-4eca-437a-a2b4-411d79916fe8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("679a3324-071c-4cc4-967f-78e06944566a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("df5b143e-4c88-4b8b-9019-c205a94c78f4"));

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("46bd8783-5d5a-41eb-9bb3-ab1ac1e1de6d"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("49a932d9-75c3-49e4-b347-21174b695e73"), "Quản trị viên hệ thống", "Admin" },
                    { new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"), "Người dùng bình thường", "User" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("48cb14cc-f6df-4298-8660-942b4ab39815"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "PasswordHash", "RefreshToken", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6"), null, "activeofficelife@gmail.com", "AQAAAAIAAYagAAAAEKozfmKyY0n6hhd94ZA3FhgIS2SlTnXdh58rcQabcIGcvirtvWF7BsR8Wfn1KgZ+Yw==", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("aeab9b22-5fed-422c-aeb3-f369a6964a24"), null, "activeoffice@gmail.com", "AQAAAAIAAYagAAAAEAhmpoG8NEwtKJ8M152vLnuV6dHDqi5V2e/rlaOcyvwj4w05UrsV+IBtFHetd6ZJmw==", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("49a932d9-75c3-49e4-b347-21174b695e73"), new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6") },
                    { new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"), new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6") },
                    { new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"), new Guid("aeab9b22-5fed-422c-aeb3-f369a6964a24") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("46bd8783-5d5a-41eb-9bb3-ab1ac1e1de6d"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("48cb14cc-f6df-4298-8660-942b4ab39815"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("49a932d9-75c3-49e4-b347-21174b695e73"), new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"), new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"), new Guid("aeab9b22-5fed-422c-aeb3-f369a6964a24") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("49a932d9-75c3-49e4-b347-21174b695e73"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ba701ab7-52c1-46f3-a01d-9c0bdb61b699"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("115316d1-f4fe-4e50-be9b-50c20ef674b6"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aeab9b22-5fed-422c-aeb3-f369a6964a24"));

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("33087857-4eca-437a-a2b4-411d79916fe8"), "Người dùng bình thường", "User" },
                    { new Guid("679a3324-071c-4cc4-967f-78e06944566a"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("9ac0d197-5234-4605-b66b-83528c9cf906"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "PasswordHash", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5"), null, "activeofficelife@gmail.com", "AQAAAAIAAYagAAAAEMzKVLZoaPLrydobScWZ2t+nWgp7Tqo2uBsXwob9ybeOEETvp0VrnWS7gB5FkXDmrg==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("df5b143e-4c88-4b8b-9019-c205a94c78f4"), null, "activeoffice@gmail.com", "AQAAAAIAAYagAAAAEO639pKJmzMEzdZJhARjSJ70HQZ6LyB0oCrLWwsCwafMQ36yrhFptJJJgamEppX6FQ==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("33087857-4eca-437a-a2b4-411d79916fe8"), new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5") },
                    { new Guid("679a3324-071c-4cc4-967f-78e06944566a"), new Guid("7144dd51-2104-465c-83fe-59af0ac54cf5") },
                    { new Guid("33087857-4eca-437a-a2b4-411d79916fe8"), new Guid("df5b143e-4c88-4b8b-9019-c205a94c78f4") }
                });
        }
    }
}
