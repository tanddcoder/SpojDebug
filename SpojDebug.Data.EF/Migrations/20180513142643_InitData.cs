using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpojDebug.Data.EF.Migrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    LastedSpojSubmissionRecorded = table.Column<long>(nullable: false),
                    SpojPasswordEncode = table.Column<string>(nullable: true),
                    SpojUserNameEncode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Problem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SpojId = table.Column<int>(nullable: true),
                    SpojLink = table.Column<string>(nullable: true),
                    SpojProblemSet = table.Column<string>(nullable: true),
                    TimeLimit = table.Column<float>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AcceptedResultCount = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    FinalResult = table.Column<int>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    SubmmissionId = table.Column<int>(nullable: false),
                    TotalResult = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    SocialNetWorkType = table.Column<int>(nullable: false),
                    SpojUserId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    Input = table.Column<string>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    Output = table.Column<string>(nullable: true),
                    ProblemId = table.Column<int>(nullable: false),
                    SeqNum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCase_Problem_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    ProblemId = table.Column<int>(nullable: true),
                    ResultId = table.Column<int>(nullable: true),
                    RunTime = table.Column<float>(nullable: false),
                    Score = table.Column<float>(nullable: false),
                    SpojId = table.Column<int>(nullable: false),
                    SubmitTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submission_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submission_Problem_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submission_Result_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Result",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeletedBy = table.Column<int>(nullable: true),
                    DeletedTime = table.Column<DateTimeOffset>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                    LastUpdatedTime = table.Column<DateTimeOffset>(nullable: true),
                    ResultId = table.Column<int>(nullable: false),
                    ResultType = table.Column<int>(nullable: false),
                    TestCaseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultDetail_Result_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Result",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultDetail_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCaseInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserId",
                table: "Account",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDetail_ResultId",
                table: "ResultDetail",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDetail_TestCaseId",
                table: "ResultDetail",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_AccountId",
                table: "Submission",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ProblemId",
                table: "Submission",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ResultId",
                table: "Submission",
                column: "ResultId",
                unique: true,
                filter: "[ResultId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestCase_ProblemId",
                table: "TestCaseInfo",
                column: "ProblemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSetting");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ResultDetail");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TestCaseInfo");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "Problem");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
