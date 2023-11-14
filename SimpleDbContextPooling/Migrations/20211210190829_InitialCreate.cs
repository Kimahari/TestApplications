using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleDbContextPooling.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Classes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Classes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Classes");
    }
}
