﻿// <auto-generated />
using System;
using Calculator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Calculator.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Calculator.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Articles", (string)null);
                });

            modelBuilder.Entity("Calculator.Models.ArticleComponent", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasPrecision(16, 3)
                        .HasColumnType("decimal(16,3)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("numberOfPieces")
                        .HasColumnType("int");

                    b.Property<decimal>("totalpriceHT")
                        .HasPrecision(16, 3)
                        .HasColumnType("decimal(16,3)");

                    b.Property<decimal>("totalpriceTTC")
                        .HasPrecision(16, 3)
                        .HasColumnType("decimal(16,3)");

                    b.Property<int>("tva")
                        .HasColumnType("int");

                    b.HasKey("ArticleId", "ComponentId");

                    b.HasIndex("ComponentId");

                    b.ToTable("ArticleComponents", (string)null);
                });

            modelBuilder.Entity("Calculator.Models.Component", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Components", (string)null);
                });

            modelBuilder.Entity("Calculator.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Calculator.Models.Article", b =>
                {
                    b.HasOne("Calculator.Models.User", null)
                        .WithMany("Articles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Calculator.Models.ArticleComponent", b =>
                {
                    b.HasOne("Calculator.Models.Article", "Article")
                        .WithMany("ArticleComponents")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Calculator.Models.Component", "Component")
                        .WithMany("ArticleComponents")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Component");
                });

            modelBuilder.Entity("Calculator.Models.Article", b =>
                {
                    b.Navigation("ArticleComponents");
                });

            modelBuilder.Entity("Calculator.Models.Component", b =>
                {
                    b.Navigation("ArticleComponents");
                });

            modelBuilder.Entity("Calculator.Models.User", b =>
                {
                    b.Navigation("Articles");
                });
#pragma warning restore 612, 618
        }
    }
}
