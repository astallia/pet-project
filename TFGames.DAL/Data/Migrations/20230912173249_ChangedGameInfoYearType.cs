using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFGames.DAL.Data.Migrations
{
    public partial class ChangedGameInfoYearType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "GameInfo",
                type: "int",
                maxLength: 4,
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "GameInfo",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                unique: true,
                filter: "[ArticleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_GameInfo_Article_ArticleId",
                table: "GameInfo",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "Tags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "GameInfo",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "GameInfo",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
