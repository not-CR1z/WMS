using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_FormParents_FormParentId",
                table: "Forms");

            migrationBuilder.RenameColumn(
                name: "FormParentId",
                table: "Forms",
                newName: "FormSubParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_FormParentId_Name",
                table: "Forms",
                newName: "IX_Forms_FormSubParentId_Name");

            migrationBuilder.AddColumn<int>(
                name: "Secuencie",
                table: "FormParents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FormSubParents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormParentId = table.Column<long>(type: "bigint", nullable: false),
                    Secuence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubParents_FormParents_FormParentId",
                        column: x => x.FormParentId,
                        principalTable: "FormParents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubParents_FormParentId_Name",
                table: "FormSubParents",
                columns: new[] { "FormParentId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_FormSubParents_FormSubParentId",
                table: "Forms",
                column: "FormSubParentId",
                principalTable: "FormSubParents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_FormSubParents_FormSubParentId",
                table: "Forms");

            migrationBuilder.DropTable(
                name: "FormSubParents");

            migrationBuilder.DropColumn(
                name: "Secuencie",
                table: "FormParents");

            migrationBuilder.RenameColumn(
                name: "FormSubParentId",
                table: "Forms",
                newName: "FormParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_FormSubParentId_Name",
                table: "Forms",
                newName: "IX_Forms_FormParentId_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_FormParents_FormParentId",
                table: "Forms",
                column: "FormParentId",
                principalTable: "FormParents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
