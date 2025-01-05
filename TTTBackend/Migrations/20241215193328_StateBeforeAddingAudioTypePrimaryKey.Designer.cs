﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TTTBackend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241215193328_StateBeforeAddingAudioTypePrimaryKey")]
    partial class StateBeforeAddingAudioTypePrimaryKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Shared.Models.AudioFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("CHAR(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AudioFiles");
                });

            modelBuilder.Entity("Shared.Models.Scene", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("CHAR(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Scenes");
                });

            modelBuilder.Entity("Shared.Models.SceneAudioFile", b =>
                {
                    b.Property<Guid>("SceneId")
                        .HasColumnType("CHAR(36)");

                    b.Property<Guid>("AudioFileId")
                        .HasColumnType("CHAR(36)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("SceneId", "AudioFileId");

                    b.HasIndex("AudioFileId");

                    b.ToTable("SceneAudioFiles");
                });

            modelBuilder.Entity("Shared.Models.Sounds.PresetSound", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<Guid?>("SoundId")
                        .HasColumnType("CHAR(36)");

                    b.Property<Guid>("SoundPresetId")
                        .HasColumnType("CHAR(36)");

                    b.HasKey("Id");

                    b.HasIndex("SoundId")
                        .IsUnique();

                    b.HasIndex("SoundPresetId");

                    b.ToTable("PresetSounds");
                });

            modelBuilder.Entity("Shared.Models.Sounds.SoundPreset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("SceneId")
                        .HasColumnType("CHAR(36)");

                    b.HasKey("Id");

                    b.HasIndex("SceneId");

                    b.ToTable("SoundPresets");
                });

            modelBuilder.Entity("Shared.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Shared.Models.AudioFile", b =>
                {
                    b.HasOne("Shared.Models.User", "User")
                        .WithMany("AudioFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shared.Models.Scene", b =>
                {
                    b.HasOne("Shared.Models.User", "User")
                        .WithMany("Scenes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shared.Models.SceneAudioFile", b =>
                {
                    b.HasOne("Shared.Models.AudioFile", "AudioFile")
                        .WithMany("SceneAudioFiles")
                        .HasForeignKey("AudioFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Scene", "Scene")
                        .WithMany("SceneAudioFiles")
                        .HasForeignKey("SceneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AudioFile");

                    b.Navigation("Scene");
                });

            modelBuilder.Entity("Shared.Models.Sounds.PresetSound", b =>
                {
                    b.HasOne("Shared.Models.AudioFile", "Sound")
                        .WithOne()
                        .HasForeignKey("Shared.Models.Sounds.PresetSound", "SoundId");

                    b.HasOne("Shared.Models.Sounds.SoundPreset", "SoundPreset")
                        .WithMany("PresetSounds")
                        .HasForeignKey("SoundPresetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sound");

                    b.Navigation("SoundPreset");
                });

            modelBuilder.Entity("Shared.Models.Sounds.SoundPreset", b =>
                {
                    b.HasOne("Shared.Models.Scene", "Scene")
                        .WithMany("SoundPresets")
                        .HasForeignKey("SceneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scene");
                });

            modelBuilder.Entity("Shared.Models.AudioFile", b =>
                {
                    b.Navigation("SceneAudioFiles");
                });

            modelBuilder.Entity("Shared.Models.Scene", b =>
                {
                    b.Navigation("SceneAudioFiles");

                    b.Navigation("SoundPresets");
                });

            modelBuilder.Entity("Shared.Models.Sounds.SoundPreset", b =>
                {
                    b.Navigation("PresetSounds");
                });

            modelBuilder.Entity("Shared.Models.User", b =>
                {
                    b.Navigation("AudioFiles");

                    b.Navigation("Scenes");
                });
#pragma warning restore 612, 618
        }
    }
}
