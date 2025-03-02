using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;


namespace BlocklistAPI.Models;

[Table( "Device" )]
[DisplayColumn( "Name" )]
[PrimaryKey( "ID" )]
[Index( "MACAddress", IsUnique = true, Name = "UC_Device_MACAddress" )]
public partial class Device
{
    [Key]
    [Column( TypeName = "int" )]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    public int ID { get; set; } = 1;

    [Length( 2, 25 )]
    [Column( TypeName = "nvarchar(25)" )]
    public string MACAddress { get; set; } = "00:00:00:00:00:00";

    /// <summary>
    /// The DeviceRemoteSites referencing this Device
    /// </summary>
    public ICollection<DeviceRemoteSite> DeviceRemoteSites { get; set; }// = [];
}

