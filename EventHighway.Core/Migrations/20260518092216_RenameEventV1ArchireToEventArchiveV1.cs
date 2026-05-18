using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameEventV1ArchireToEventArchiveV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives");

            migrationBuilder.RenameTable(
                name: "EventV1Archives",
                newName: "EventArchiveV1");

            migrationBuilder.RenameColumn(
                name: "EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                newName: "EventArchiveV1Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                newName: "IX_ListenerEventV1Archives_EventArchiveV1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV1Archives_EventArchiveV1_EventArchiveV1Id",
                table: "ListenerEventV1Archives",
                column: "EventArchiveV1Id",
                principalTable: "EventArchiveV1",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventArchiveV1Id",
                table: "ListenerEventV1Archives");

            migrationBuilder.RenameColumn(
                name: "EventArchiveV1Id",
                table: "ListenerEventV1Archives",
                newName: "EventV1ArchiveId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV1Archives_EventArchiveV1Id",
                table: "ListenerEventV1Archives",
                newName: "IX_ListenerEventV1Archives_EventV1ArchiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                column: "EventV1ArchiveId",
                principalTable: "EventV1Archives",
                principalColumn: "Id");
        }
    }
}
