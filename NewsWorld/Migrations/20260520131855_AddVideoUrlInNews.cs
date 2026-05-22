using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsWorld.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlInNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "News");
        }
    }
}
