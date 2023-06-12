using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Althea.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_StorageObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VoiceId",
                table: "Message",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StorageObject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bucket = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageObject", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_VoiceId",
                table: "Message",
                column: "VoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_StorageObject_VoiceId",
                table: "Message",
                column: "VoiceId",
                principalTable: "StorageObject",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_StorageObject_VoiceId",
                table: "Message");

            migrationBuilder.DropTable(
                name: "StorageObject");

            migrationBuilder.DropIndex(
                name: "IX_Message_VoiceId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "VoiceId",
                table: "Message");
        }
    }
}
