using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class InitialCreate : Migration
    // มี2 method ใน class
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable( // สร้าง table ชื่อ "User"
                name: "Users",
                columns: table => new
                { // มี 2 colum คือ Id, UserName
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true), // คือ Autoincrement ทุกครั้งที่มี record ใหม่เพิ่มเข้ามา
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id); // และ Id เป็น primary key
                });
        }
    // เป็น method ที่ทำการลบ migration (Down method)
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( // ทำการลบ table ชื่อ Users
                name: "Users");
        }
    }
}
