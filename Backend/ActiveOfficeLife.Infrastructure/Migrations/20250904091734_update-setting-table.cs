using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatesettingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "GoogleClientId",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleClientSecretId",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleFolderId",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleToken",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "GoogleClientId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "GoogleClientSecretId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "GoogleFolderId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "GoogleToken",
                table: "Settings");

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
    }
}
