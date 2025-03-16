using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Models;

/// <summary>
/// Ties a device to a remote site for downloads
/// </summary>
[Table( "DeviceRemoteSite" )]
[PrimaryKey( "ID" )]
[Index( "DeviceID", IsUnique = false, Name = "IX_DeviceRemoteSite_DeviceID" )]
[Index( "RemoteSiteID", IsUnique = false, Name = "IX_DeviceRemoteSite_RemoteSiteID" )]
[Index( "DeviceID", [ "RemoteSiteID" ], AllDescending = false, IsUnique = true, Name = "IX_DeviceRemoteSite_DeviceID_RemoteSiteID" )]
public partial class DeviceRemoteSite
{
    /// <summary>
    /// The ID of the device/remote site relationship
    /// </summary>
    [Key]
    [Column( "ID", TypeName = "int" )]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    public int ID { get; set; }

    /// <summary>
    /// The details of the device
    /// </summary>
    [ForeignKey( "DeviceID" )]
    [DeleteBehavior( DeleteBehavior.Restrict )]
    public virtual Device? Device { get; set; }

    /// <summary>
    /// The ID of the device
    /// </summary>
    [Required]
    public int DeviceID { get; set; }

    /// <summary>
    /// The details of the remote site
    /// </summary>
    [ForeignKey( "RemoteSiteID" )]
    [DeleteBehavior( DeleteBehavior.Restrict )]
    public virtual RemoteSite? RemoteSite { get; set; }

    /// <summary>
    /// The ID of the remote site
    /// </summary>
    [Required]
    public int RemoteSiteID { get; set; }

    /// <summary>
    /// The date and time of the most recent download triggered by this device
    /// </summary>
    // [Column( "LastDownloaded", TypeName = "DATETIME" )]
    // [AllowNull( )]
    public DateTime LastDownloaded { get; internal set; }
}

