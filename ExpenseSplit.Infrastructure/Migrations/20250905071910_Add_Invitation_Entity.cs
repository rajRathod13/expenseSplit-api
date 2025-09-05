using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Invitation_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InvitedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.InvitationId);
                    table.ForeignKey(
                        name: "FK_Invitations_GroupDetails_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupDetails",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_User_InvitedUserId",
                        column: x => x.InvitedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_User_InviterId",
                        column: x => x.InviterId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_GroupId_InvitedUserId_Status",
                table: "Invitations",
                columns: new[] { "GroupId", "InvitedUserId", "Status" },
                unique: true,
                filter: "[Status] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InvitedUserId",
                table: "Invitations",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InviterId",
                table: "Invitations",
                column: "InviterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitations");
        }
    }
}
