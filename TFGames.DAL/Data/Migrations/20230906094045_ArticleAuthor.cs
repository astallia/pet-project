using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFGames.DAL.Data.Migrations
{
    public partial class ArticleAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Article_ArticleId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Article",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Article_AuthorId",
                table: "Article",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_AuthorId",
                table: "Article",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Article_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_AuthorId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Article_ArticleId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Article_AuthorId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Article_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");
        }
    }
}
