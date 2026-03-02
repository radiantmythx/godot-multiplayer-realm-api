using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealmAuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMapsAndMapLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    scene_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_playable = table.Column<bool>(type: "boolean", nullable: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: false),
                    min_level = table.Column<int>(type: "integer", nullable: true),
                    max_level = table.Column<int>(type: "integer", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "map_links",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from_map_id = table.Column<int>(type: "integer", nullable: false),
                    to_map_id = table.Column<int>(type: "integer", nullable: false),
                    link_kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    label = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_map_links_maps_from_map_id",
                        column: x => x.from_map_id,
                        principalTable: "maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_map_links_maps_to_map_id",
                        column: x => x.to_map_id,
                        principalTable: "maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_links_from_map_id_is_enabled_sort_order",
                table: "map_links",
                columns: new[] { "from_map_id", "is_enabled", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_map_links_from_map_id_to_map_id_link_kind_label",
                table: "map_links",
                columns: new[] { "from_map_id", "to_map_id", "link_kind", "label" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_map_links_to_map_id",
                table: "map_links",
                column: "to_map_id");

            migrationBuilder.CreateIndex(
                name: "IX_maps_is_playable_is_hidden_sort_order",
                table: "maps",
                columns: new[] { "is_playable", "is_hidden", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_maps_slug",
                table: "maps",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_links");

            migrationBuilder.DropTable(
                name: "maps");
        }
    }
}
