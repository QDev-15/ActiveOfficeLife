using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addusertoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId",
                table: "UserToken",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToken");

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
    }
}
