using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expenseTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingGoalToTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_SavingGoals_SavingGoalId",
                table: "Transfers");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_SavingGoals_SavingGoalId",
                table: "Transfers",
                column: "SavingGoalId",
                principalTable: "SavingGoals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_SavingGoals_SavingGoalId",
                table: "Transfers");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_SavingGoals_SavingGoalId",
                table: "Transfers",
                column: "SavingGoalId",
                principalTable: "SavingGoals",
                principalColumn: "Id");
        }
    }
}
