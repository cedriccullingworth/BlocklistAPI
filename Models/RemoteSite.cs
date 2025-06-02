using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Models;

/// <summary>
/// Details of a source for blocklist downloads
/// </summary>
[Table( "RemoteSite" )]
[DisplayColumn( "Name" )]
[PrimaryKey( "ID" )]
[Index( "Name", additionalPropertyNames: [ "SiteUrl" ], IsUnique = true, Name = "IX_RemoteSite_Name" )]
[Index( "FileTypeID", Name = "IX_RemoteSite_FileTypeID" )]
public class RemoteSite
{
    /// <summary>
    /// The identity value of the remote site
    /// </summary>
    [Key]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    [Column( TypeName = "int" )]
    public int ID { get; set; } = 0;

    /// <summary>
    /// The name of the remote site
    /// </summary>
    [Length( 2, 50 )]
    [Column( TypeName = "nvarchar(128)" )]
    public required string Name { get; set; }

    // public DateTime? LastDownloaded { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The home Url of the remote site
    /// </summary>
    [Length( 2, 255 )]
    [Column( TypeName = "nvarchar(255)" )]
    public string SiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// A comma-separated array of download file Urls
    /// </summary>
    [Required]
    [Length( 2, 4000 )]
    [Column( TypeName = "nvarchar(4000)" )]
    public required string FileUrls { get; set; }

    /// <summary>
    /// A list of file paths of the remote site
    /// Not currently in use
    /// </summary>
    public IList<string> FilePaths
    {
        get
        {
            return this.FileUrls.Split( ',' )
                           .Select( s => s.Trim( ) )
                           .ToList( );
        }
    }

    /// <summary>
    /// The date and time of the most recent download triggered by any device
    /// </summary>
    public DateTime LastDownloaded { get; set; }
    //{
    //    get
    //    {
    //        return DateTime.MinValue; // Subqueries.GetLastDownloadDateTime( ID ); // this.DeviceRemoteSites.Select( s => s.LastDownloaded ).Max( );
    //    }

    //    internal set => LastDownloaded = value;

    //    //internal set
    //    //{
    //    //    LastDownloaded = value;
    //    //}
    //}

    /// <summary>
    /// The ID of the file type applicable for remote site
    /// </summary>
    public int FileTypeID { get; set; } = 1;

    /// <summary>
    /// The tuple of the file type for remote site
    /// </summary>
    [ForeignKey( "FileTypeID" )]
    [DeleteBehavior( DeleteBehavior.Restrict )]
    public virtual FileType? FileType { get; set; }

    /// <summary>
    /// The active status of the remote site
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// The minimum number of minutes to wait since the last download
    /// </summary>
    public int MinimumIntervalMinutes { get; set; } = 30;

    /// <summary>
    /// The key value for the remote site, used for authentication or other purposes
    /// </summary>
    public string? KeyValue { get; set; } = null;

    // <summary>
    // The DeviceRemoteSites referencing this RemoteSite
    // </summary>
    // public ICollection<DeviceRemoteSite> DeviceRemoteSites { get; set; }// = [];
}
