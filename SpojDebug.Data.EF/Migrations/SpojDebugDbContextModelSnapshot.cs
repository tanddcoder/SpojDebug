﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SpojDebug.Core.Constant;
using SpojDebug.Data.EF.Contexts;
using System;

namespace SpojDebug.Data.EF.Migrations
{
    [DbContext(typeof(SpojDebugDbContext))]
    partial class SpojDebugDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Account.AccountEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<string>("Phone");

                    b.Property<int>("SpojUserId");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.AdminSetting.AdminSettingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<string>("SpojPasswordEncode");

                    b.Property<string>("SpojUserNameEncode");

                    b.HasKey("Id");

                    b.ToTable("AdminSetting");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Problem.ProblemEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<DateTime?>("DownloadTestCaseTime");

                    b.Property<bool?>("IsDownloadedTestCase");

                    b.Property<bool?>("IsSkip");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<string>("Name");

                    b.Property<int?>("SpojId");

                    b.Property<string>("SpojProblemSet");

                    b.Property<float?>("TimeLimit");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Problem");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Result.ResultEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<int>("Result");

                    b.Property<int>("SubmissionId");

                    b.Property<int>("TestCaseSeq");

                    b.HasKey("Id");

                    b.HasIndex("SubmissionId");

                    b.ToTable("Result");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Submission.SubmissionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<DateTime?>("DownloadedTime");

                    b.Property<bool?>("IsDownloadedInfo");

                    b.Property<bool?>("IsNotHaveEnoughInfo");

                    b.Property<string>("Language");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<int?>("ProblemId");

                    b.Property<float>("RunTime");

                    b.Property<float>("Score");

                    b.Property<int>("SpojId");

                    b.Property<DateTime>("SubmitTime");

                    b.Property<int?>("TotalResult");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ProblemId");

                    b.ToTable("Submission");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.TestCase.TestCaseInfoEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedTime");

                    b.Property<DateTime?>("DeletedTime");

                    b.Property<DateTime?>("LastUpdatedTime");

                    b.Property<string>("Path");

                    b.Property<int>("ProblemId");

                    b.Property<int>("TotalTestCase");

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.ToTable("TestCaseInfo");
                });

            modelBuilder.Entity("SpojDebug.Core.User.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SpojDebug.Core.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SpojDebug.Core.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SpojDebug.Core.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SpojDebug.Core.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Account.AccountEntity", b =>
                {
                    b.HasOne("SpojDebug.Core.User.ApplicationUser", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Result.ResultEntity", b =>
                {
                    b.HasOne("SpojDebug.Core.Entities.Submission.SubmissionEntity", "Submission")
                        .WithMany("Results")
                        .HasForeignKey("SubmissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.Submission.SubmissionEntity", b =>
                {
                    b.HasOne("SpojDebug.Core.Entities.Account.AccountEntity", "Account")
                        .WithMany("Submissions")
                        .HasForeignKey("AccountId");

                    b.HasOne("SpojDebug.Core.Entities.Problem.ProblemEntity", "Problem")
                        .WithMany("Submissions")
                        .HasForeignKey("ProblemId");
                });

            modelBuilder.Entity("SpojDebug.Core.Entities.TestCase.TestCaseInfoEntity", b =>
                {
                    b.HasOne("SpojDebug.Core.Entities.Problem.ProblemEntity", "Problem")
                        .WithMany("TestCaseInfos")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
