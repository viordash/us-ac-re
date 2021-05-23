﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Migrations {
	[DbContext(typeof(ApplicationDbContext))]
	[Migration("20210523190113_UpdateIdentityObjects")]
	partial class UpdateIdentityObjects {
		protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation("Relational:MaxIdentifierLength", 64)
				.HasAnnotation("ProductVersion", "5.0.4");

			modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.DeviceFlowCodes", b => {
				b.Property<string>("UserCode")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<string>("ClientId")
					.IsRequired()
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<DateTime>("CreationTime")
					.HasColumnType("datetime");

				b.Property<string>("Data")
					.IsRequired()
					.HasMaxLength(50000)
					.HasColumnType("text");

				b.Property<string>("Description")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<string>("DeviceCode")
					.IsRequired()
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<DateTime?>("Expiration")
					.IsRequired()
					.HasColumnType("datetime");

				b.Property<string>("SessionId")
					.HasMaxLength(100)
					.HasColumnType("varchar(100)");

				b.Property<string>("SubjectId")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.HasKey("UserCode");

				b.HasIndex("DeviceCode")
					.IsUnique();

				b.HasIndex("Expiration");

				b.ToTable("DeviceCodes");
			});

			modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.PersistedGrant", b => {
				b.Property<string>("Key")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<string>("ClientId")
					.IsRequired()
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<DateTime?>("ConsumedTime")
					.HasColumnType("datetime");

				b.Property<DateTime>("CreationTime")
					.HasColumnType("datetime");

				b.Property<string>("Data")
					.IsRequired()
					.HasMaxLength(50000)
					.HasColumnType("text");

				b.Property<string>("Description")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<DateTime?>("Expiration")
					.HasColumnType("datetime");

				b.Property<string>("SessionId")
					.HasMaxLength(100)
					.HasColumnType("varchar(100)");

				b.Property<string>("SubjectId")
					.HasMaxLength(200)
					.HasColumnType("varchar(200)");

				b.Property<string>("Type")
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnType("varchar(50)");

				b.HasKey("Key");

				b.HasIndex("Expiration");

				b.HasIndex("SubjectId", "ClientId", "Type");

				b.HasIndex("SubjectId", "SessionId", "Type");

				b.ToTable("PersistedGrants");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("int");

				b.Property<string>("ClaimType")
					.HasColumnType("text");

				b.Property<string>("ClaimValue")
					.HasColumnType("text");

				b.Property<string>("RoleId")
					.IsRequired()
					.HasColumnType("varchar(767)");

				b.HasKey("Id");

				b.HasIndex("RoleId");

				b.ToTable("AspNetRoleClaims");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("int");

				b.Property<string>("ClaimType")
					.HasColumnType("text");

				b.Property<string>("ClaimValue")
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("varchar(100)");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserClaims");
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityRole", b => {
				b.Property<string>("Id")
					.HasColumnType("varchar(767)");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("text");

				b.Property<string>("Name")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("NormalizedName")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedName")
					.IsUnique()
					.HasDatabaseName("RoleNameIndex");

				b.ToTable("AspNetRoles");
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserLogin", b => {
				b.Property<string>("LoginProvider")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("ProviderKey")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("ProviderDisplayName")
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("varchar(100)");

				b.HasKey("LoginProvider", "ProviderKey");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserLogins");
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserRole", b => {
				b.Property<string>("UserId")
					.HasColumnType("varchar(100)");

				b.Property<string>("RoleId")
					.HasColumnType("varchar(767)");

				b.HasKey("UserId", "RoleId");

				b.HasIndex("RoleId");

				b.ToTable("AspNetUserRoles");
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserToken", b => {
				b.Property<string>("UserId")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("LoginProvider")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("Name")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("Value")
					.HasColumnType("text");

				b.HasKey("UserId", "LoginProvider", "Name");

				b.ToTable("AspNetUserTokens");
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationUser", b => {
				b.Property<string>("Id")
					.HasMaxLength(100)
					.HasColumnType("varchar(100)");

				b.Property<int>("AccessFailedCount")
					.HasColumnType("int");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("text");

				b.Property<string>("Email")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<bool>("EmailConfirmed")
					.HasColumnType("tinyint(1)");

				b.Property<bool>("LockoutEnabled")
					.HasColumnType("tinyint(1)");

				b.Property<DateTimeOffset?>("LockoutEnd")
					.HasColumnType("timestamp");

				b.Property<string>("NormalizedEmail")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("NormalizedUserName")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.Property<string>("PasswordHash")
					.HasColumnType("text");

				b.Property<string>("PhoneNumber")
					.HasColumnType("text");

				b.Property<bool>("PhoneNumberConfirmed")
					.HasColumnType("tinyint(1)");

				b.Property<string>("SecurityStamp")
					.HasColumnType("text");

				b.Property<bool>("TwoFactorEnabled")
					.HasColumnType("tinyint(1)");

				b.Property<string>("UserName")
					.HasMaxLength(256)
					.HasColumnType("varchar(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedEmail")
					.HasDatabaseName("EmailIndex");

				b.HasIndex("NormalizedUserName")
					.IsUnique()
					.HasDatabaseName("UserNameIndex");

				b.ToTable("AspNetUsers");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationIdentityRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserLogin", b => {
				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserRole", b => {
				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationIdentityRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("UsAcRe.Web.Server.Identity.ApplicationIdentityUserToken", b => {
				b.HasOne("UsAcRe.Web.Server.Identity.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});
#pragma warning restore 612, 618
		}
	}
}
