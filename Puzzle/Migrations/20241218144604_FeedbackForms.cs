using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Migrations
{
    public partial class FeedbackForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedbackForms",
                schema: "Puzzle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    DesignQualityVote = table.Column<int>(type: "int", nullable: true),
                    ComplianceOfTheOrderWithTheDesignVote = table.Column<int>(type: "int", nullable: true),
                    AppropriateApproachOfTheDesignerVote = table.Column<int>(type: "int", nullable: true),
                    PriceVote = table.Column<int>(type: "int", nullable: true),
                    AppropriateTreatmentOfManagementAndOfficeWorkersVote = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackForms_AspNetUsers_DesignerId",
                        column: x => x.DesignerId,
                        principalSchema: "Puzzle",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackForms_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Puzzle",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackForms_CustomerId",
                schema: "Puzzle",
                table: "FeedbackForms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackForms_DesignerId",
                schema: "Puzzle",
                table: "FeedbackForms",
                column: "DesignerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackForms",
                schema: "Puzzle");
        }
    }
}
