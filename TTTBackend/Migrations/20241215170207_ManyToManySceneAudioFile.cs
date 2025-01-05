using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManySceneAudioFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SceneAudioFiles",
                columns: table => new
                {
                    SceneId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    AudioFileId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneAudioFiles", x => new { x.SceneId, x.AudioFileId });
                    table.ForeignKey(
                        name: "FK_SceneAudioFiles_AudioFiles_AudioFileId",
                        column: x => x.AudioFileId,
                        principalTable: "AudioFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SceneAudioFiles_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SceneAudioFiles_AudioFileId",
                table: "SceneAudioFiles",
                column: "AudioFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SceneAudioFiles");
        }
    }
}
