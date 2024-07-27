﻿// <auto-generated />
using System;
using HotelManagementApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelManagementApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240727002210_AddConfirmationNumberToBooking")]
    partial class AddConfirmationNumberToBooking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HotelManagementApp.Repository.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ConfirmationNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("BookingId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("HotelManagementApp.Repository.Hotel", b =>
                {
                    b.Property<int>("HotelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HotelId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("HotelId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Hotels");

                    b.HasData(
                        new
                        {
                            HotelId = 1,
                            Address = "123 Street A",
                            Name = "Hotel A"
                        },
                        new
                        {
                            HotelId = 2,
                            Address = "456 Street B",
                            Name = "Hotel B"
                        });
                });

            modelBuilder.Entity("HotelManagementApp.Repository.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomId"));

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.HasKey("RoomId");

                    b.HasIndex("HotelId");

                    b.ToTable("Rooms");

                    b.HasData(
                        new
                        {
                            RoomId = 1,
                            HotelId = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            RoomId = 2,
                            HotelId = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            RoomId = 3,
                            HotelId = 2,
                            IsAvailable = true
                        });
                });

            modelBuilder.Entity("HotelManagementApp.Repository.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Name = "John Doe"
                        },
                        new
                        {
                            UserId = 2,
                            Name = "Jane Smith"
                        });
                });

            modelBuilder.Entity("HotelManagementApp.Repository.Booking", b =>
                {
                    b.HasOne("HotelManagementApp.Repository.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelManagementApp.Repository.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HotelManagementApp.Repository.Room", b =>
                {
                    b.HasOne("HotelManagementApp.Repository.Hotel", "Hotel")
                        .WithMany("Rooms")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("HotelManagementApp.Repository.Hotel", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("HotelManagementApp.Repository.User", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
