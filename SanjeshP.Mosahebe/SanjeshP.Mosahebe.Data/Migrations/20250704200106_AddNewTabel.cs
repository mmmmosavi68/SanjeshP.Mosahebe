using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddNewTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    ProfileImageUrl = table.Column<string>(nullable: true),
                    Bio = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Slug = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamNews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    PublishStartDate = table.Column<DateTime>(nullable: true),
                    PublishEndDate = table.Column<DateTime>(nullable: true),
                    Summary = table.Column<string>(maxLength: 500, nullable: true),
                    FullArticle = table.Column<string>(nullable: true),
                    FullArticleLink = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(maxLength: 150, nullable: false),
                    MetaTitle = table.Column<string>(maxLength: 60, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 150, nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    AuthorId1 = table.Column<int>(nullable: true),
                    ReviewedById = table.Column<Guid>(nullable: true),
                    ReviewedById1 = table.Column<int>(nullable: true),
                    ReviewedAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsFeatured = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    IsInSlider = table.Column<bool>(nullable: false),
                    SliderStartDate = table.Column<DateTime>(nullable: true),
                    SliderEndDate = table.Column<DateTime>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ViewCount = table.Column<long>(nullable: false),
                    LikeCount = table.Column<int>(nullable: false),
                    DislikeCount = table.Column<int>(nullable: false),
                    ShareCount = table.Column<int>(nullable: false),
                    CommentCount = table.Column<int>(nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    Region = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamNews_Authors_AuthorId1",
                        column: x => x.AuthorId1,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamNews_Authors_ReviewedById1",
                        column: x => x.ReviewedById1,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    NewsId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(maxLength: 200, nullable: false),
                    Url = table.Column<string>(nullable: false),
                    MediaType = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_ExamNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "ExamNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    Content = table.Column<string>(maxLength: 1000, nullable: false),
                    NewsId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_ExamNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "ExamNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    NewsId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false),
                    UniqueVisitors = table.Column<int>(nullable: false),
                    AvgTimeOnPage = table.Column<double>(nullable: false),
                    CommentsCount = table.Column<int>(nullable: false),
                    LikeCount = table.Column<int>(nullable: false),
                    DislikeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsStatistics_ExamNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "ExamNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsTags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    NewsId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsTags_ExamNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "ExamNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    NewsId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_ExamNews_NewsId",
                        column: x => x.NewsId,
                        principalTable: "ExamNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_NewsId",
                table: "Attachments",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_NewsId",
                table: "Comments",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamNews_AuthorId1",
                table: "ExamNews",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExamNews_ReviewedById1",
                table: "ExamNews",
                column: "ReviewedById1");

            migrationBuilder.CreateIndex(
                name: "IX_NewsStatistics_NewsId",
                table: "NewsStatistics",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTags_NewsId",
                table: "NewsTags",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTags_TagId",
                table: "NewsTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_NewsId",
                table: "Ratings",
                column: "NewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "NewsStatistics");

            migrationBuilder.DropTable(
                name: "NewsTags");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ExamNews");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
