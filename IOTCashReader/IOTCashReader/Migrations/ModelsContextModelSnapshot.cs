﻿// <auto-generated />
using System;
using IOTCashReader.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IOTCashReader.Migrations
{
    [DbContext(typeof(ModelsContext))]
    partial class ModelsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IOTCashReader.Models.ActivityLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Status");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("ActivityLog");
                });

            modelBuilder.Entity("IOTCashReader.Models.Cashout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<double>("Total");

                    b.HasKey("Id");

                    b.ToTable("Cashout");
                });

            modelBuilder.Entity("IOTCashReader.Models.Credit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("IOTCashReader.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount");

                    b.Property<string>("Counts");

                    b.Property<string>("Response");

                    b.Property<string>("Type");

                    b.Property<int?>("UserId");

                    b.Property<bool>("isCompleted");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("IOTCashReader.Models.Safe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AcceptorActive");

                    b.Property<int>("Bill10");

                    b.Property<int>("Bill100");

                    b.Property<int>("Bill20");

                    b.Property<int>("Bill50");

                    b.Property<string>("SafeName");

                    b.Property<string>("SerialNumber");

                    b.HasKey("Id");

                    b.HasIndex("SerialNumber")
                        .IsUnique()
                        .HasFilter("[SerialNumber] IS NOT NULL");

                    b.ToTable("Safe");
                });

            modelBuilder.Entity("IOTCashReader.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.Property<bool>("isAdmin");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("User");
                });

            modelBuilder.Entity("IOTCashReader.Models.UserCredit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreditId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreditId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCredit");
                });

            modelBuilder.Entity("IOTCashReader.Models.UserWithdrawal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("UserId");

                    b.Property<int?>("WithdrawalId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WithdrawalId");

                    b.ToTable("UserWithdrawal");
                });

            modelBuilder.Entity("IOTCashReader.Models.Withdrawal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.ToTable("Withdrawal");
                });

            modelBuilder.Entity("IOTCashReader.Models.Request", b =>
                {
                    b.HasOne("IOTCashReader.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IOTCashReader.Models.UserCredit", b =>
                {
                    b.HasOne("IOTCashReader.Models.Credit", "Credit")
                        .WithMany()
                        .HasForeignKey("CreditId");

                    b.HasOne("IOTCashReader.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IOTCashReader.Models.UserWithdrawal", b =>
                {
                    b.HasOne("IOTCashReader.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.HasOne("IOTCashReader.Models.Withdrawal", "Withdrawal")
                        .WithMany()
                        .HasForeignKey("WithdrawalId");
                });
#pragma warning restore 612, 618
        }
    }
}
