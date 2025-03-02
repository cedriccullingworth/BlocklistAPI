using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using BlocklistAPI.Context;

using Microsoft.EntityFrameworkCore.Migrations;

using MySql.EntityFrameworkCore.Metadata;

using MySqlConnector;

#nullable disable

namespace BlocklistAPI.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up( MigrationBuilder migrationBuilder )
    {
        migrationBuilder.AlterDatabase( )
            .Annotation( "MySql:CharSet", "utf8mb4" );

        string sql = "SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM information_schema.TABLES WHERE TABLE_SCHEMA LIKE 'sbsdosnr_ocart' AND TABLE_TYPE LIKE 'BASE TABLE' AND TABLE_NAME = 'FileType';";
        var queryResults = ExecuteMySqlQuery( sql );

        if ( !queryResults.Any( ) ) // Doesn't exist
        {
            migrationBuilder.CreateTable(
                name: "FileType",
                columns: table => new
                {
                    ID = table.Column<int>( type: "int", nullable: false )
                        .Annotation( "MySql:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn ),
                    Name = table.Column<string>( type: "nvarchar(50)", nullable: false ),
                    Description = table.Column<string>( type: "nvarchar(255)", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_FileType", x => x.ID );
                } )
                .Annotation( "MySql:CharSet", "utf8mb4" );
        }

        sql = "SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM information_schema.TABLES WHERE TABLE_SCHEMA LIKE 'sbsdosnr_ocart' AND TABLE_TYPE LIKE 'BASE TABLE' AND TABLE_NAME = 'RemoteSite';";
        queryResults = ExecuteMySqlQuery( sql );

        if ( !queryResults.Any( ) ) // Doesn't exist
        {
            migrationBuilder.CreateTable(
            name: "RemoteSite",
            columns: table => new
            {
                ID = table.Column<int>( type: "int", nullable: false )
                    .Annotation( "MySql:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn ),
                Name = table.Column<string>( type: "nvarchar(128)", nullable: false ),
                LastDownloaded = table.Column<DateTime>( type: "datetime(6)", nullable: true ),
                SiteUrl = table.Column<string>( type: "nvarchar(255)", nullable: false ),
                FileUrls = table.Column<string>( type: "nvarchar(4000)", nullable: false ),
                FileTypeID = table.Column<int>( type: "int", nullable: true ),
                Active = table.Column<bool>( type: "tinyint(1)", nullable: false ),
                MinimumIntervalMinutes = table.Column<int>( type: "int", nullable: false )
            },
            constraints: table =>
            {
                table.PrimaryKey( "PK_RemoteSite", x => x.ID );
                table.ForeignKey(
                    name: "FK_RemoteSite_FileType_FileTypeID",
                    column: x => x.FileTypeID,
                    principalTable: "FileType",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Restrict );
            } )
            .Annotation( "MySql:CharSet", "utf8mb4" );
        }

        migrationBuilder.CreateIndex(
            name: "IX_RemoteSite_FileTypeID",
            table: "RemoteSite",
            column: "FileTypeID" );

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
            name: "RemoteSite" );

        migrationBuilder.DropTable(
            name: "FileType" );
    }

    private List<List<string>> ExecuteMySqlQuery( string sql )
    {
        List<List<string>> results = [];
        string connectionString = ocartDbContext.GetConnectionString( );
        using ( DbConnection connection1 = new MySqlConnection( connectionString ) )
        {
            using ( DbCommand command1 = connection1.CreateCommand( ) )
            {
                command1.CommandText = sql;
                connection1.Open( );
                using ( DbDataReader reader = command1.ExecuteReader( ) )
                {
                    if ( reader.HasRows )
                    {
                        while ( reader.Read( ) )
                        {
                            List<string> rowData = [];
                            for ( int i = 0; i < reader.FieldCount; i++ )
                            {
                                rowData.Add( reader.GetString( i ) );
                            }

                            results.Add( rowData );
                        }
                    }
                }
            }
        }

        return results;
    }
}
