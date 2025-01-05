using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class MovedAudioTypeToSceneAudioFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "AudioFiles");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SceneAudioFiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "SceneAudioFiles");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AudioFiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
