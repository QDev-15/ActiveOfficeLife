using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0daa0446-8569-46fa-9aed-671c7558248e"), "Quản trị viên hệ thống", "Admin" },
                    { new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"), "Người dùng bình thường", "User" },
                    { new Guid("40291fb7-65c7-451b-af7d-4f17b71b785e"), "Người đọc chỉ comment", "Comment" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("8de5e0a0-b183-4c5c-94f5-b2b1ac572969"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "PasswordHash", "RefreshToken", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("2e1f3376-2db7-4645-b112-d0f833037c73"), null, "activeofficelife@gmail.com", "AQAAAAIAAYagAAAAEBZRG45lwmAOdkQHtIJYc5d4nzHK52jbtuvCZoQ9vJxWNCdpcYVfhzj3ZTv29srtGw==", null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("68b1856d-9e4d-4749-b4f6-c992ef07e28f"), null, "activeoffice@gmail.com", "AQAAAAIAAYagAAAAEHOEub9gaR8VFFAWXtf7hLxa2RWCdRn4Rg7lOQYx2xKHUSbmuu48Bl5Pk5f6j4FgXQ==", null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("0daa0446-8569-46fa-9aed-671c7558248e"), new Guid("2e1f3376-2db7-4645-b112-d0f833037c73") },
                    { new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"), new Guid("2e1f3376-2db7-4645-b112-d0f833037c73") },
                    { new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"), new Guid("68b1856d-9e4d-4749-b4f6-c992ef07e28f") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("40291fb7-65c7-451b-af7d-4f17b71b785e"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("8de5e0a0-b183-4c5c-94f5-b2b1ac572969"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0daa0446-8569-46fa-9aed-671c7558248e"), new Guid("2e1f3376-2db7-4645-b112-d0f833037c73") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"), new Guid("2e1f3376-2db7-4645-b112-d0f833037c73") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"), new Guid("68b1856d-9e4d-4749-b4f6-c992ef07e28f") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0daa0446-8569-46fa-9aed-671c7558248e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d51b941-5c52-424c-9d5a-e70a1a6872d5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2e1f3376-2db7-4645-b112-d0f833037c73"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("68b1856d-9e4d-4749-b4f6-c992ef07e28f"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

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
    }
}
