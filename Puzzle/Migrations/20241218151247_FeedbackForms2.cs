using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Migrations
{
    public partial class FeedbackForms2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeVote",
                schema: "Puzzle",
                table: "FeedbackForms",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTimeVote",
                schema: "Puzzle",
                table: "FeedbackForms");
        }
    }
}
