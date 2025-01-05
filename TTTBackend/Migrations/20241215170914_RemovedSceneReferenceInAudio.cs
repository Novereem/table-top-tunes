using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSceneReferenceInAudio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioFiles_Scenes_SceneId",
                table: "AudioFiles");

            migrationBuilder.DropIndex(
                name: "IX_AudioFiles_SceneId",
                table: "AudioFiles");

            migrationBuilder.DropColumn(
                name: "SceneId",
                table: "AudioFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SceneId",
                table: "AudioFiles",
                type: "CHAR(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_SceneId",
                table: "AudioFiles",
                column: "SceneId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioFiles_Scenes_SceneId",
                table: "AudioFiles",
                column: "SceneId",
                principalTable: "Scenes",
                principalColumn: "Id");
        }
    }
}
