using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlocklistAPI.Migrations
{
    /// <inheritdoc />
    public partial class MySQLInit : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AlterDatabase( )
                .Annotation( "MySQL:Charset", "utf8mb4" );

            migrationBuilder.AlterTable(
                name: "Device" )
                //columns: table => new
                //{
                //    ID = table.Column<int>(type: "int", nullable: false)
                //        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                //    MACAddress = table.Column<string>(type: "nvarchar(25)", nullable: false)
                //},
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_Device", x => x.ID);
                //})
                .Annotation( "MySQL:Charset", "utf8mb4" );

            migrationBuilder.AlterTable(
                name: "FileType" )
                //columns: table => new
                //{
                //    ID = table.Column<int>(type: "int", nullable: false)
                //        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                //    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                //    Description = table.Column<string>(type: "nvarchar(255)", nullable: false)
                //},
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_FileType", x => x.ID);
                //})
                .Annotation( "MySQL:Charset", "utf8mb4" );

            migrationBuilder.AlterTable(
                name: "RemoteSite" )
                //columns: table => new
                //{
                //    ID = table.Column<int>(type: "int", nullable: false)
                //        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                //    Name = table.Column<string>(type: "nvarchar(128)", nullable: false),
                //    SiteUrl = table.Column<string>(type: "nvarchar(255)", nullable: false),
                //    FileUrls = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                //    LastDownloaded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                //    FileTypeID = table.Column<int>(type: "int", nullable: true),
                //    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                //    MinimumIntervalMinutes = table.Column<int>(type: "int", nullable: false)
                //},
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_RemoteSite", x => x.ID);
                //    table.ForeignKey(
                //        name: "FK_RemoteSite_FileType_FileTypeID",
                //        column: x => x.FileTypeID,
                //        principalTable: "FileType",
                //        principalColumn: "ID",
                //        onDelete: ReferentialAction.Restrict);
                //})
                .Annotation( "MySQL:Charset", "utf8mb4" );

            migrationBuilder.AlterTable(
                name: "DeviceRemoteSite" )
                //columns: table => new
                //{
                //    ID = table.Column<int>(type: "int", nullable: false)
                //        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                //    DeviceID = table.Column<int>(type: "int", nullable: false),
                //    RemoteSiteID = table.Column<int>(type: "int", nullable: false),
                //    LastDownloaded = table.Column<DateTime>(type: "DATETIME", nullable: false)
                //},
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_DeviceRemoteSite", x => x.ID);
                //    table.ForeignKey(
                //        name: "FK_DeviceRemoteSite_Device_DeviceID",
                //        column: x => x.DeviceID,
                //        principalTable: "Device",
                //        principalColumn: "ID",
                //        onDelete: ReferentialAction.Restrict);
                //    table.ForeignKey(
                //        name: "FK_DeviceRemoteSite_RemoteSite_RemoteSiteID",
                //        column: x => x.RemoteSiteID,
                //        principalTable: "RemoteSite",
                //        principalColumn: "ID",
                //        onDelete: ReferentialAction.Restrict);
                //})
                .Annotation( "MySQL:Charset", "utf8mb4" );

            migrationBuilder.DropIndex( name: "UC_Device_MACAddress", table: "Device" );
            migrationBuilder.CreateIndex(
                name: "UC_Device_MACAddress",
                table: "Device",
                column: "MACAddress",
                unique: true );

            migrationBuilder.DropIndex( name: "IX_DeviceRemoteSite_DeviceID", table: "DeviceRemoteSite" );
            migrationBuilder.CreateIndex(
                name: "IX_DeviceRemoteSite_DeviceID",
                table: "DeviceRemoteSite",
                column: "DeviceID" );

            migrationBuilder.DropIndex( name: "IX_DeviceRemoteSite_DeviceID_RemoteSiteID", table: "DeviceRemoteSite" );
            migrationBuilder.CreateIndex(
                name: "IX_DeviceRemoteSite_DeviceID_RemoteSiteID",
                table: "DeviceRemoteSite",
                columns: new[] { "DeviceID", "RemoteSiteID" },
                unique: true );

            migrationBuilder.DropIndex( name: "IX_DeviceRemoteSite_RemoteSiteID", table: "DeviceRemoteSite" );
            migrationBuilder.CreateIndex(
                name: "IX_DeviceRemoteSite_RemoteSiteID",
                table: "DeviceRemoteSite",
                column: "RemoteSiteID" );

            migrationBuilder.DropIndex( name: "IX_FileType_Name", table: "FileType" );
            migrationBuilder.CreateIndex(
                name: "IX_FileType_Name",
                table: "FileType",
                column: "Name",
                unique: true );

            migrationBuilder.DropIndex( name: "IX_RemoteSite_FileTypeID", table: "RemoteSite" );
            migrationBuilder.CreateIndex(
                name: "IX_RemoteSite_FileTypeID",
                table: "RemoteSite",
                column: "FileTypeID" );

            migrationBuilder.DropIndex( name: "IX_RemoteSite_Name", table: "RemoteSite" );
            migrationBuilder.CreateIndex(
                name: "IX_RemoteSite_Name",
                table: "RemoteSite",
                columns: new[] { "Name", "SiteUrl" },
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "DeviceRemoteSite" );

            migrationBuilder.DropTable(
                name: "Device" );

            migrationBuilder.DropTable(
                name: "RemoteSite" );

            migrationBuilder.DropTable(
                name: "FileType" );
        }
    }
}
