using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFGames.DAL.Data.Migrations
{
    public partial class ImageAndContentStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Article_ArticleId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "MainImage",
                table: "Article");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "GameInfo",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ContentId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MainImageId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArticleContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MainImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleImages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                unique: true,
                filter: "[ArticleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Article_ContentId",
                table: "Article",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_MainImageId",
                table: "Article",
                column: "MainImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_ArticleContents_ContentId",
                table: "Article",
                column: "ContentId",
                principalTable: "ArticleContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article",
                column: "MainImageId",
                principalTable: "ArticleImages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Article_ArticleId",
                table: "Tags",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_ArticleContents_ContentId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Article_ArticleId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "ArticleContents");

            migrationBuilder.DropTable(
                name: "ArticleImages");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo");

            migrationBuilder.DropIndex(
                name: "IX_Article_ContentId",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_MainImageId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Article");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "GameInfo",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "MainImage",
                table: "Article",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Article_ArticleId",
                table: "Tags",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
