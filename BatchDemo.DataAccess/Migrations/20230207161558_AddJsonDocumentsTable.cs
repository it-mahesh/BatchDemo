using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchDemo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JsonDocuments",
                columns: table => new
                {
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonDocuments", x => x.BatchId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JsonDocuments");
        }
    }
}
