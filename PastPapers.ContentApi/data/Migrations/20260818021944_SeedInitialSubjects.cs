using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PastPapers.ContentApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "content",
                table: "subjects",
                columns: new[] { "id", "created_at", "name", "slug" },
                values: new object[,]
                {
                    { new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Physical Sciences", "physical-sciences" },
                    { new Guid("d8475ca5-e465-4c16-a445-e9f043054976"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mathematics", "mathematics" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "content",
                table: "subjects",
                keyColumn: "id",
                keyValue: new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "subjects",
                keyColumn: "id",
                keyValue: new Guid("d8475ca5-e465-4c16-a445-e9f043054976"));
        }
    }
}
