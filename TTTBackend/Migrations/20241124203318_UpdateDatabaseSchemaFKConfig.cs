using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTTBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchemaFKConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PresetSounds_AudioFiles_Id",
                table: "PresetSounds");

            migrationBuilder.DropColumn(
                name: "SoundType",
                table: "PresetSounds");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "AudioFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "SoundId",
                table: "PresetSounds",
                type: "CHAR(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "AudioFiles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PresetSounds_SoundId",
                table: "PresetSounds",
                column: "SoundId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PresetSounds_AudioFiles_SoundId",
                table: "PresetSounds",
                column: "SoundId",
                principalTable: "AudioFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PresetSounds_AudioFiles_SoundId",
                table: "PresetSounds");

            migrationBuilder.DropIndex(
                name: "IX_PresetSounds_SoundId",
                table: "PresetSounds");

            migrationBuilder.DropColumn(
                name: "SoundId",
                table: "PresetSounds");

            migrationBuilder.AddColumn<string>(
                name: "SoundType",
                table: "PresetSounds",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "AudioFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "AudioFiles",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_PresetSounds_AudioFiles_Id",
                table: "PresetSounds",
                column: "Id",
                principalTable: "AudioFiles",
                principalColumn: "Id");
        }
    }
}
