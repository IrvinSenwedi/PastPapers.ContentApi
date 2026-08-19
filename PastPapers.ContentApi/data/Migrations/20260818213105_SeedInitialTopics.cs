using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PastPapers.ContentApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "content",
                table: "topics",
                columns: new[] { "id", "created_at", "display_order", "grade", "name", "slug", "subject_id" },
                values: new object[,]
                {
                    { new Guid("2a9eaed1-51a4-47d1-958a-c87982cdcf4d"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, (short)12, "Electricity and Magnetism", "electricity-and-magnetism", new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc") },
                    { new Guid("2fe0d037-2212-4134-8dd5-59000f040d90"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, (short)12, "Algebra, Equations, and Inequalities", "algebra-equations-and-inequalities", new Guid("d8475ca5-e465-4c16-a445-e9f043054976") },
                    { new Guid("50461d4e-9824-4d5c-ae08-3fcaa46bfd9f"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, (short)12, "Newtonian Mechanics", "newtonian-mechanics", new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc") },
                    { new Guid("62bbbf58-791b-40dd-8bb4-1294a04a5897"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, (short)12, "Doppler Effect", "doppler-effect", new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc") },
                    { new Guid("6472c568-1ac8-447d-8526-66196e7a164b"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, (short)12, "Electrodynamics", "electrodynamics", new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc") },
                    { new Guid("70022de8-3747-4a58-a872-378cf299b9e1"), new DateTimeOffset(new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, (short)12, "Optical Phenomena", "optical-phenomena", new Guid("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("2a9eaed1-51a4-47d1-958a-c87982cdcf4d"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("2fe0d037-2212-4134-8dd5-59000f040d90"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("50461d4e-9824-4d5c-ae08-3fcaa46bfd9f"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("62bbbf58-791b-40dd-8bb4-1294a04a5897"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("6472c568-1ac8-447d-8526-66196e7a164b"));

            migrationBuilder.DeleteData(
                schema: "content",
                table: "topics",
                keyColumn: "id",
                keyValue: new Guid("70022de8-3747-4a58-a872-378cf299b9e1"));
        }
    }
}
