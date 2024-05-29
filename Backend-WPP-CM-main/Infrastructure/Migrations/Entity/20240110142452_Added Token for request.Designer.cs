﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations.Entity
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240110142452_Added Token for request")]
    partial class AddedTokenforrequest
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DonainModel.Car", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("licensePlate")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("seats")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("licensePlate")
                        .IsUnique();

                    b.ToTable("Cars");

                    b.HasData(
                        new
                        {
                            id = 1,
                            brand = "brand",
                            imageUrl = "url",
                            isAvailable = true,
                            licensePlate = "999-XX-9",
                            location = "location",
                            model = "model",
                            seats = 5
                        });
                });

            modelBuilder.Entity("DonainModel.Request", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("creationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("reservationid")
                        .HasColumnType("int");

                    b.Property<int>("seats")
                        .HasColumnType("int");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("reservationid");

                    b.HasIndex("userid");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("DonainModel.Reservation", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("arrivalDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("carid")
                        .HasColumnType("int");

                    b.Property<DateTime>("creationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("departureDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("destination")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("seats")
                        .HasColumnType("int");

                    b.Property<int>("userid")
                        .HasColumnType("int");

                    b.Property<bool>("willReturn")
                        .HasColumnType("bit");

                    b.HasKey("id");

                    b.HasIndex("carid");

                    b.HasIndex("userid");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("DonainModel.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("emailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("employeeNr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("hasLicense")
                        .HasColumnType("bit");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("role")
                        .HasColumnType("int");

                    b.Property<string>("securityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DonainModel.Request", b =>
                {
                    b.HasOne("DonainModel.Reservation", "reservation")
                        .WithMany("requests")
                        .HasForeignKey("reservationid");

                    b.HasOne("DonainModel.User", "user")
                        .WithMany("requests")
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("reservation");

                    b.Navigation("user");
                });

            modelBuilder.Entity("DonainModel.Reservation", b =>
                {
                    b.HasOne("DonainModel.Car", "car")
                        .WithMany("reservations")
                        .HasForeignKey("carid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DonainModel.User", "user")
                        .WithMany("reservations")
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("car");

                    b.Navigation("user");
                });

            modelBuilder.Entity("DonainModel.Car", b =>
                {
                    b.Navigation("reservations");
                });

            modelBuilder.Entity("DonainModel.Reservation", b =>
                {
                    b.Navigation("requests");
                });

            modelBuilder.Entity("DonainModel.User", b =>
                {
                    b.Navigation("requests");

                    b.Navigation("reservations");
                });
#pragma warning restore 612, 618
        }
    }
}