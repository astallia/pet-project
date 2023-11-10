using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFGames.DAL.Data.Migrations
{
    public partial class ApplicationSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article");

            migrationBuilder.AlterColumn<Guid>(
                name: "MainImageId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DbProvider = table.Column<int>(type: "int", nullable: false),
                    UseBlob = table.Column<bool>(type: "bit", nullable: false),
                    UseCache = table.Column<bool>(type: "bit", nullable: false),
                    CompressImages = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article",
                column: "MainImageId",
                principalTable: "ArticleImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article");

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.AlterColumn<Guid>(
                name: "MainImageId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_ArticleImages_MainImageId",
                table: "Article",
                column: "MainImageId",
                principalTable: "ArticleImages",
                principalColumn: "Id");
        }
    }
}
