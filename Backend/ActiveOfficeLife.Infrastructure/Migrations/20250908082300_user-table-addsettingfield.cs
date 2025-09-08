using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class usertableaddsettingfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4c83f242-0193-405e-9e81-169e8181dd0f"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("96a06a0e-d5a3-4aa2-a2e4-a278004244a6"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("1906695f-8d29-4872-b72c-ab272088ea29"), new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("737a9a31-640b-4837-b035-79b79c9281c0"), new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("1906695f-8d29-4872-b72c-ab272088ea29"), new Guid("892e4063-c0c4-43cb-8360-9e4575635ad5") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1906695f-8d29-4872-b72c-ab272088ea29"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("737a9a31-640b-4837-b035-79b79c9281c0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("892e4063-c0c4-43cb-8360-9e4575635ad5"));

            migrationBuilder.AddColumn<string>(
                name: "SettingId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "SettingId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Medias");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("1906695f-8d29-4872-b72c-ab272088ea29"), "Người dùng bình thường", "User" },
                    { new Guid("4c83f242-0193-405e-9e81-169e8181dd0f"), "Người đọc chỉ comment", "Comment" },
                    { new Guid("737a9a31-640b-4837-b035-79b79c9281c0"), "Quản trị viên hệ thống", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "GoogleClientId", "GoogleClientSecretId", "GoogleFolderId", "GoogleToken", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("96a06a0e-d5a3-4aa2-a2e4-a278004244a6"), null, null, null, null, null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "FullName", "PasswordHash", "PhoneNumber", "RefreshToken", "Status", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4"), null, "activeofficelife@gmail.com", null, "AQAAAAIAAYagAAAAEJsWnN8EZX2K0VdjdhTI7fi6f9Pr5Cnv7sk1wYe7mNMT5XG4UA6ItyNKggPj0EKPxg==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("892e4063-c0c4-43cb-8360-9e4575635ad5"), null, "activeoffice@gmail.com", null, "AQAAAAIAAYagAAAAEMgw5RwM6R/gS8fqlG9Hvhf2esCq39soI0soPWInyviH73vLJDOkeQdlyRBR33ue1Q==", null, null, "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("1906695f-8d29-4872-b72c-ab272088ea29"), new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4") },
                    { new Guid("737a9a31-640b-4837-b035-79b79c9281c0"), new Guid("663e96ed-e1ad-4793-b85d-ea970ea898f4") },
                    { new Guid("1906695f-8d29-4872-b72c-ab272088ea29"), new Guid("892e4063-c0c4-43cb-8360-9e4575635ad5") }
                });
        }
    }
}
