using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatecatetorytype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1cc58526-2aa3-4a6c-ab9b-d49a82aabac6"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("ef54a74a-ad7f-4b1f-9f70-ea808db65f41"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"), new Guid("51d8be44-f9b7-4bef-8138-4eeaa34ecd62") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"), new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ae41398a-a16a-450c-a61f-9f17be64767d"), new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ae41398a-a16a-450c-a61f-9f17be64767d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("51d8be44-f9b7-4bef-8138-4eeaa34ecd62"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryTypeId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("723cb144-7bab-4f48-9166-cb4b17608d2b"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("cc336059-8b60-4fde-880b-64f508547850"), "Người dùng bình thường", "User" },
                    { new Guid("ce93730c-3e65-497f-8439-d40455f62e7c"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "GoogleClientId", "GoogleClientSecretId", "GoogleFolderId", "GoogleToken", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("cd4c9292-602c-4bde-82eb-2fa4a20b794e"), null, null, null, null, null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "FullName", "PasswordHash", "PhoneNumber", "RefreshToken", "SettingId", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("5660f8e0-4210-4820-8ea6-94f4268d8e36"), null, "activeoffice@gmail.com", null, "AQAAAAIAAYagAAAAEBkJefu3i8Nfaq7G8aB0R2g70mt7V5ZRVTwFxiqyLHLX5pHalFfpWjQfDE/OXq9xlg==", null, null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" },
                    { new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30"), null, "activeofficelife@gmail.com", null, "AQAAAAIAAYagAAAAEHcU6Cj7ykIMhIm/LVQja/kWAiNssAJemYt1NAF5U31VcUNeWEuviJB7gkiCZLqavA==", null, null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("cc336059-8b60-4fde-880b-64f508547850"), new Guid("5660f8e0-4210-4820-8ea6-94f4268d8e36") },
                    { new Guid("cc336059-8b60-4fde-880b-64f508547850"), new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30") },
                    { new Guid("ce93730c-3e65-497f-8439-d40455f62e7c"), new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryTypeId",
                table: "Categories",
                column: "CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_CategoryTypes_CategoryTypeId",
                table: "Categories",
                column: "CategoryTypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CategoryTypes_CategoryTypeId",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "CategoryTypes");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryTypeId",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("723cb144-7bab-4f48-9166-cb4b17608d2b"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("cd4c9292-602c-4bde-82eb-2fa4a20b794e"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("cc336059-8b60-4fde-880b-64f508547850"), new Guid("5660f8e0-4210-4820-8ea6-94f4268d8e36") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("cc336059-8b60-4fde-880b-64f508547850"), new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ce93730c-3e65-497f-8439-d40455f62e7c"), new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("cc336059-8b60-4fde-880b-64f508547850"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ce93730c-3e65-497f-8439-d40455f62e7c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5660f8e0-4210-4820-8ea6-94f4268d8e36"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69065b8e-8a7e-47b0-b9f1-ce9cdf35de30"));

            migrationBuilder.DropColumn(
                name: "CategoryTypeId",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("1cc58526-2aa3-4a6c-ab9b-d49a82aabac6"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"), "Người dùng bình thường", "User" },
                    { new Guid("ae41398a-a16a-450c-a61f-9f17be64767d"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "GoogleClientId", "GoogleClientSecretId", "GoogleFolderId", "GoogleToken", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("ef54a74a-ad7f-4b1f-9f70-ea808db65f41"), null, null, null, null, null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "FullName", "PasswordHash", "PhoneNumber", "RefreshToken", "SettingId", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("51d8be44-f9b7-4bef-8138-4eeaa34ecd62"), null, "activeoffice@gmail.com", null, "AQAAAAIAAYagAAAAEMbNaTNBR1bJ5nfHEiZL56iU2IlAntJLZgzgLX0CvfGoyvQqQebY2xQ9GviqZgmocA==", null, null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" },
                    { new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6"), null, "activeofficelife@gmail.com", null, "AQAAAAIAAYagAAAAEDRrtfuSIMdY27qdsMrpksZpvYv+lwDIuK1c5EwBRO+dTG9IWgd3jKGVipt6/Z+e+g==", null, null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"), new Guid("51d8be44-f9b7-4bef-8138-4eeaa34ecd62") },
                    { new Guid("6e1208a5-4872-4f84-b899-747b2b9e2980"), new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6") },
                    { new Guid("ae41398a-a16a-450c-a61f-9f17be64767d"), new Guid("95df4e65-a6c7-4397-9b73-58e6a8d66bb6") }
                });
        }
    }
}
