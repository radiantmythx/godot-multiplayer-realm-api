using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealmAuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "accounts",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "accounts",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "accounts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "accounts",
                newName: "password_hash");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_Username",
                table: "accounts",
                newName: "IX_accounts_username");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_Email",
                table: "accounts",
                newName: "IX_accounts_email");

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    class_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.id);
                    table.ForeignKey(
                        name: "FK_characters_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_account_id_name",
                table: "characters",
                columns: new[] { "account_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "accounts",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "accounts",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "accounts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "accounts",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_username",
                table: "accounts",
                newName: "IX_accounts_Username");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_email",
                table: "accounts",
                newName: "IX_accounts_Email");
        }
    }
}
