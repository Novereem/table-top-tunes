using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class AdjustModelsForAudioFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SceneId",
                table: "AudioFiles",
                type: "CHAR(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AudioFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AudioFiles",
                type: "CHAR(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_SceneId",
                table: "AudioFiles",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_UserId",
                table: "AudioFiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioFiles_Scenes_SceneId",
                table: "AudioFiles",
                column: "SceneId",
                principalTable: "Scenes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioFiles_Users_UserId",
                table: "AudioFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioFiles_Scenes_SceneId",
                table: "AudioFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioFiles_Users_UserId",
                table: "AudioFiles");

            migrationBuilder.DropIndex(
                name: "IX_AudioFiles_SceneId",
                table: "AudioFiles");

            migrationBuilder.DropIndex(
                name: "IX_AudioFiles_UserId",
                table: "AudioFiles");

            migrationBuilder.DropColumn(
                name: "SceneId",
                table: "AudioFiles");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AudioFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AudioFiles");
        }
    }
}
