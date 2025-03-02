using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Models;

[Table( "DeviceRemoteSite" )]
[PrimaryKey( "ID" )]
[Index( "DeviceID", IsUnique = false, Name = "IX_DeviceRemoteSite_DeviceID" )]
[Index( "RemoteSiteID", IsUnique = false, Name = "IX_DeviceRemoteSite_RemoteSiteID" )]
[Index( "DeviceID", [ "RemoteSiteID" ], AllDescending = false, IsUnique = true, Name = "IX_DeviceRemoteSite_DeviceID_RemoteSiteID" )]
public partial class DeviceRemoteSite
{
    [Key]
    [Column( "ID", TypeName = "int" )]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    public int ID { get; set; }

    //[Column( TypeName = "int" )]
    [ForeignKey( "DeviceID" )]
    [DeleteBehavior( DeleteBehavior.Restrict )]
    public virtual Device Device { get; set; }

    public int DeviceID { get; set; }

    //[Column( TypeName = "int" )]
    [ForeignKey( "RemoteSiteID" )]
    [DeleteBehavior( DeleteBehavior.Restrict )]
    public virtual RemoteSite RemoteSite { get; set; }

    public int RemoteSiteID { get; set; }

    /// <summary>
    /// The date and time of the most recent download triggered by this device
    /// </summary>
    // [Column( "LastDownloaded", TypeName = "DATETIME" )]
    // [AllowNull( )]
    public DateTime LastDownloaded { get; internal set; }
}

