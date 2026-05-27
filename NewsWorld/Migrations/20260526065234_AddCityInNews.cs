using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsWorld.Migrations
{
    /// <inheritdoc />
    public partial class AddCityInNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "News",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_CityId",
                table: "News",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Cities_CityId",
                table: "News",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Cities_CityId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_CityId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "News");
        }
    }
}
