using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealmAuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMapSpawnEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "map_spawn_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    map_id = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    min_pack_size = table.Column<int>(type: "integer", nullable: false),
                    max_pack_size = table.Column<int>(type: "integer", nullable: false),
                    min_packs = table.Column<int>(type: "integer", nullable: false),
                    max_packs = table.Column<int>(type: "integer", nullable: false),
                    min_level = table.Column<int>(type: "integer", nullable: true),
                    max_level = table.Column<int>(type: "integer", nullable: true),
                    tags = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_spawn_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_map_spawn_entries_maps_map_id",
                        column: x => x.map_id,
                        principalTable: "maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_spawn_entries_map_id_type_id",
                table: "map_spawn_entries",
                columns: new[] { "map_id", "type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_map_spawn_entries_map_id_weight",
                table: "map_spawn_entries",
                columns: new[] { "map_id", "weight" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_spawn_entries");
        }
    }
}
