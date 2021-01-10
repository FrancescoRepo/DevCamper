using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevCamper.Migrations
{
    public partial class AddedPictureToBootcampModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Bootcamps",
                nullable: false,
                defaultValue: new byte[] {  });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Bootcamps");
        }
    }
}
