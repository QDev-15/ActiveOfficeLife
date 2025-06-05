using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ActiveOfficeLife.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("4d305d6c-3002-4b07-b9c0-566013606288"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"), new Guid("309d9ed3-b7f3-4318-84f7-72d33001aba0") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("640fbd89-10f6-4a67-a55c-5d9270f09e72"), new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"), new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("640fbd89-10f6-4a67-a55c-5d9270f09e72"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("309d9ed3-b7f3-4318-84f7-72d33001aba0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8"));

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("640fbd89-10f6-4a67-a55c-5d9270f09e72"), "Quản trị viên hệ thống", "Admin" },
                    { new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"), "Người dùng bình thường", "User" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Address", "Email", "Logo", "Name", "PhoneNumber" },
                values: new object[] { new Guid("4d305d6c-3002-4b07-b9c0-566013606288"), null, null, null, "Active Office Life", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "Email", "PasswordHash", "Token", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("309d9ed3-b7f3-4318-84f7-72d33001aba0"), null, "activeoffice@gmail.com", "AQAAAAIAAYagAAAAECzKUAXphp3YxaB2+GyqBGKGzgkgTvCNjzSs1ThKPC0QCgZnA/zRYiF/a8UgVw0EWw==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user" },
                    { new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8"), null, "activeofficelife@gmail.com", "AQAAAAIAAYagAAAAEPWRdoOdiK/7Wcm8Ufc2eImEc84J4Mgc4MPC3oizzajWuweoSDtfpmQB10sOYkzpZg==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"), new Guid("309d9ed3-b7f3-4318-84f7-72d33001aba0") },
                    { new Guid("640fbd89-10f6-4a67-a55c-5d9270f09e72"), new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8") },
                    { new Guid("ef6372d1-7103-46f7-9286-0864f62c2c96"), new Guid("bd946d81-1375-42bd-8a9f-fad64ca37ca8") }
                });
        }
    }
}
