using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenDesign.Migrations
{
    /// <inheritdoc />
    public partial class allcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CategoryId",
                table: "Students",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Categorys_CategoryId",
                table: "Students",
                column: "CategoryId",
                principalTable: "Categorys",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Categorys_CategoryId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_CategoryId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Students");
        }
    }
}
