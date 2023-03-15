﻿// <auto-generated />
using System;
using Lekkerbek.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    [DbContext(typeof(LekkerbekContext))]
    partial class LekkerbekContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Lekkerbek.Web.Models.Chef", b =>
                {
                    b.Property<int>("ChefId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChefId"));

                    b.Property<string>("ChefName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChefId");

                    b.ToTable("Chefs");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LoyaltyScore")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PreferredDishId")
                        .HasColumnType("int");

                    b.HasKey("CustomerId");

                    b.HasIndex("PreferredDishId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Dish", b =>
                {
                    b.Property<int>("DishId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DishId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("DishId");

                    b.ToTable("Dishes");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderID"));

                    b.Property<int?>("CustomerID")
                        .HasColumnType("int");

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<bool>("Finished")
                        .HasColumnType("bit");

                    b.Property<int?>("TimeSlotID")
                        .HasColumnType("int");

                    b.HasKey("OrderID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("TimeSlotID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.OrderLine", b =>
                {
                    b.Property<int>("OrderLineID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderLineID"));

                    b.Property<int>("DishAmount")
                        .HasColumnType("int");

                    b.Property<int?>("DishID")
                        .HasColumnType("int");

                    b.Property<string>("ExtraDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderID")
                        .HasColumnType("int");

                    b.HasKey("OrderLineID");

                    b.HasIndex("DishID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderLines");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.PreferredDish", b =>
                {
                    b.Property<int>("PreferredDishId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PreferredDishId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PreferredDishId");

                    b.ToTable("PreferredDishes");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.TimeSlot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ChefId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTimeSlot")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChefId");

                    b.ToTable("TimeSlots");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Customer", b =>
                {
                    b.HasOne("Lekkerbek.Web.Models.PreferredDish", "PreferredDish")
                        .WithMany("Customers")
                        .HasForeignKey("PreferredDishId");

                    b.Navigation("PreferredDish");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Order", b =>
                {
                    b.HasOne("Lekkerbek.Web.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerID");

                    b.HasOne("Lekkerbek.Web.Models.TimeSlot", "TimeSlot")
                        .WithMany("Orders")
                        .HasForeignKey("TimeSlotID");

                    b.Navigation("Customer");

                    b.Navigation("TimeSlot");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.OrderLine", b =>
                {
                    b.HasOne("Lekkerbek.Web.Models.Dish", "Dish")
                        .WithMany("OrderLines")
                        .HasForeignKey("DishID");

                    b.HasOne("Lekkerbek.Web.Models.Order", "Order")
                        .WithMany("OrderLines")
                        .HasForeignKey("OrderID");

                    b.Navigation("Dish");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.TimeSlot", b =>
                {
                    b.HasOne("Lekkerbek.Web.Models.Chef", "Chef")
                        .WithMany("TimeSlot")
                        .HasForeignKey("ChefId");

                    b.Navigation("Chef");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Chef", b =>
                {
                    b.Navigation("TimeSlot");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Dish", b =>
                {
                    b.Navigation("OrderLines");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.Order", b =>
                {
                    b.Navigation("OrderLines");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.PreferredDish", b =>
                {
                    b.Navigation("Customers");
                });

            modelBuilder.Entity("Lekkerbek.Web.Models.TimeSlot", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
