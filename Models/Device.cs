using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Models;

/// <summary>
/// A user's device represented by its MAC address
/// </summary>
[Table( "Device" )]
[DisplayColumn( "Name" )]
[PrimaryKey( "ID" )]
[Index( "MACAddress", IsUnique = true, Name = "UC_Device_MACAddress" )]
public partial class Device
{
    /// <summary>
    /// The identity value of the device
    /// </summary>
    [Key]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    //    [Column( TypeName = "int" )]
    public int ID { get; set; } = 1;

    /// <summary>
    /// The MAC address of the device
    /// </summary>
    [Length( 2, 25 )]
    [Column( TypeName = "nvarchar(25)" )]
    public string MACAddress { get; set; } = "00:00:00:00:00:00";

    /// <summary>
    /// The DeviceRemoteSites referencing this Device
    /// </summary>
    public ICollection<DeviceRemoteSite> DeviceRemoteSites { get; set; } = [];
}

