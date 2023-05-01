using System;
using Althea.Infrastructure.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Althea.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Own = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    TotalUsage = table.Column<int>(type: "integer", nullable: false),
                    LastSendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit = table.Column<DeletableAudit>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    PrevMessageId = table.Column<long>(type: "bigint", nullable: true),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Usage = table.Column<int>(type: "integer", nullable: false),
                    Audit = table.Column<DeletableAudit>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_Message_PrevMessageId",
                        column: x => x.PrevMessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    CompletionId = table.Column<long>(type: "bigint", nullable: false),
                    PromptUsage = table.Column<int>(type: "integer", nullable: false),
                    CompletionUsage = table.Column<int>(type: "integer", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorInfo = table.Column<string>(type: "text", nullable: true),
                    Audit = table.Column<BasicAudit>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLog_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestLog_Message_CompletionId",
                        column: x => x.CompletionId,
                        principalTable: "Message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageRequestLog",
                columns: table => new
                {
                    PromptsId = table.Column<long>(type: "bigint", nullable: false),
                    PromptsLogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRequestLog", x => new { x.PromptsId, x.PromptsLogId });
                    table.ForeignKey(
                        name: "FK_MessageRequestLog_Message_PromptsId",
                        column: x => x.PromptsId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageRequestLog_RequestLog_PromptsLogId",
                        column: x => x.PromptsLogId,
                        principalTable: "RequestLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatId",
                table: "Message",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_PrevMessageId",
                table: "Message",
                column: "PrevMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRequestLog_PromptsLogId",
                table: "MessageRequestLog",
                column: "PromptsLogId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLog_ChatId",
                table: "RequestLog",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLog_CompletionId",
                table: "RequestLog",
                column: "CompletionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageRequestLog");

            migrationBuilder.DropTable(
                name: "RequestLog");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Chat");
        }
    }
}
