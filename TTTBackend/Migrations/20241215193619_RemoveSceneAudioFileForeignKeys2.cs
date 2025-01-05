using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSceneAudioFileForeignKeys2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SceneAudioFiles_AudioFiles_AudioFileId",
                table: "sceneaudiofiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneAudioFiles_Scenes_SceneId",
                table: "sceneaudiofiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
