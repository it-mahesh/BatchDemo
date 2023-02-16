using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchDemo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchIdColumnInFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchId",
                table: "Files",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "Files");
        }
    }
}
