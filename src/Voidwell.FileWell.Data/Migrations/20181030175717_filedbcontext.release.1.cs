using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voidwell.FileWell.Data.Migrations
{
    public partial class filedbcontextrelease1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file",
                columns: table => new
                {
                    file_id = table.Column<Guid>(nullable: false),
                    file_name = table.Column<string>(nullable: true),
                    file_type = table.Column<string>(nullable: true),
                    file_mime_type = table.Column<string>(nullable: true),
                    file_size = table.Column<long>(nullable: false),
                    tags = table.Column<List<string>>(nullable: true),
                    upload_user_id = table.Column<Guid>(nullable: false),
                    uploaded_date = table.Column<DateTime>(nullable: false),
                    delete_user_id = table.Column<Guid>(nullable: true),
                    deleted_date = table.Column<DateTime>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file", x => x.file_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file");
        }
    }
}
