﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TPaper.DeliveryRequest;

namespace TPaper.DeliveryRequest
{
    [DbContext(typeof(PaperDbContext))]
    [Migration("20211101220615_DeliveryRequestResponseMigration")]
    partial class DeliveryRequestResponseMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TPaper.DeliveryRequest.DeliveryRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int?>("DeliveryId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductCode")
                        .HasColumnType("int");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("DeliveryRequest");
                });

            modelBuilder.Entity("TPaper.DeliveryRequest.DeliveryRequest", b =>
                {
                    b.OwnsOne("TPaper.DeliveryRequest.Response", "Response", b1 =>
                        {
                            b1.Property<int>("DeliveryRequestId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Status")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("DeliveryRequestId");

                            b1.ToTable("DeliveryRequest");

                            b1.WithOwner()
                                .HasForeignKey("DeliveryRequestId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}