﻿// <auto-generated />
using System;
using Bonapio4Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bonapio4Database.Migrations
{
    [DbContext(typeof(StudentContext))]
    partial class StudentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Bonapio4Database.Club", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("RoleId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("StudentServerId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("StudentUserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Name", "ServerId");

                    b.HasIndex("StudentUserId", "StudentServerId");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("Bonapio4Database.Profile", b =>
                {
                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("Xp")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ServerId");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Bonapio4Database.Server", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Logs")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Prefix")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<ulong>("Spam")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("Bonapio4Database.Student", b =>
                {
                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Grade")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Surname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserId", "ServerId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Bonapio4Database.Club", b =>
                {
                    b.HasOne("Bonapio4Database.Student", null)
                        .WithMany("Club")
                        .HasForeignKey("StudentUserId", "StudentServerId");
                });
#pragma warning restore 612, 618
        }
    }
}
