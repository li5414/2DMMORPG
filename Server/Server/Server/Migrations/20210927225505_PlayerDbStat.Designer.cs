﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.DB;

namespace Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210927225505_PlayerDbStat")]
    partial class PlayerDbStat
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Server.DB.AccountDb", b =>
                {
                    b.Property<int>("AccountDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AccountDbId");

                    b.HasIndex("AccountName")
                        .IsUnique()
                        .HasFilter("[AccountName] IS NOT NULL");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Server.DB.ItemDb", b =>
                {
                    b.Property<int>("ItemDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<bool>("Equipped")
                        .HasColumnType("bit");

                    b.Property<int?>("OwnerDbId")
                        .HasColumnType("int");

                    b.Property<int>("PosX")
                        .HasColumnType("int");

                    b.Property<int>("PosY")
                        .HasColumnType("int");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<int>("Slot")
                        .HasColumnType("int");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int");

                    b.HasKey("ItemDbId");

                    b.HasIndex("OwnerDbId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("Server.DB.PlayerDb", b =>
                {
                    b.Property<int>("PlayerDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountDbId")
                        .HasColumnType("int");

                    b.Property<int>("Attack")
                        .HasColumnType("int");

                    b.Property<int>("Defence")
                        .HasColumnType("int");

                    b.Property<int>("Hp")
                        .HasColumnType("int");

                    b.Property<int>("Int")
                        .HasColumnType("int");

                    b.Property<int>("JobClassType")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("MaxHp")
                        .HasColumnType("int");

                    b.Property<int>("MaxMp")
                        .HasColumnType("int");

                    b.Property<int>("Mp")
                        .HasColumnType("int");

                    b.Property<string>("PlayerName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PosX")
                        .HasColumnType("int");

                    b.Property<int>("PosY")
                        .HasColumnType("int");

                    b.Property<float>("Speed")
                        .HasColumnType("real");

                    b.Property<int>("StatPoints")
                        .HasColumnType("int");

                    b.Property<int>("Str")
                        .HasColumnType("int");

                    b.Property<int>("TotalExp")
                        .HasColumnType("int");

                    b.HasKey("PlayerDbId");

                    b.HasIndex("AccountDbId");

                    b.HasIndex("PlayerName")
                        .IsUnique()
                        .HasFilter("[PlayerName] IS NOT NULL");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("Server.DB.QuestDb", b =>
                {
                    b.Property<int>("QuestDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CurrentNumber")
                        .HasColumnType("int");

                    b.Property<int>("OwnerDbId")
                        .HasColumnType("int");

                    b.Property<int>("QuestState")
                        .HasColumnType("int");

                    b.Property<int>("TmeplateId")
                        .HasColumnType("int");

                    b.HasKey("QuestDbId");

                    b.HasIndex("OwnerDbId");

                    b.ToTable("Quest");
                });

            modelBuilder.Entity("Server.DB.SkillDb", b =>
                {
                    b.Property<int>("SkillDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OwnerDbId")
                        .HasColumnType("int");

                    b.Property<string>("SkillLevelData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SkillPoints")
                        .HasColumnType("int");

                    b.HasKey("SkillDbId");

                    b.HasIndex("OwnerDbId")
                        .IsUnique();

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("Server.DB.ItemDb", b =>
                {
                    b.HasOne("Server.DB.PlayerDb", "Owner")
                        .WithMany("Items")
                        .HasForeignKey("OwnerDbId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Server.DB.PlayerDb", b =>
                {
                    b.HasOne("Server.DB.AccountDb", "Account")
                        .WithMany("Players")
                        .HasForeignKey("AccountDbId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Server.DB.QuestDb", b =>
                {
                    b.HasOne("Server.DB.PlayerDb", "Owner")
                        .WithMany("Quests")
                        .HasForeignKey("OwnerDbId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Server.DB.SkillDb", b =>
                {
                    b.HasOne("Server.DB.PlayerDb", "Owner")
                        .WithOne("Skills")
                        .HasForeignKey("Server.DB.SkillDb", "OwnerDbId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Server.DB.AccountDb", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Server.DB.PlayerDb", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("Quests");

                    b.Navigation("Skills");
                });
#pragma warning restore 612, 618
        }
    }
}
