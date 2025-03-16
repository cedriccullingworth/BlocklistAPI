using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Models;

/// <summary>
/// File types
/// </summary>
[Table( "FileType" )]
[DisplayColumn( "Name" )]
[PrimaryKey( "ID" )]
[Index( "Name", IsUnique = true, Name = "IX_FileType_Name" )]
public partial class FileType
{
    /// <summary>
    /// The identity value of the device
    /// </summary>
    [Key]
    [Column( TypeName = "int" )]
    [DatabaseGenerated( DatabaseGeneratedOption.Identity )]
    public int ID { get; set; } = 1;

    /// <summary>
    /// The name of the file type
    /// </summary>
    [Length( 2, 50 )]
    [Column( TypeName = "nvarchar(50)" )]
    public string Name { get; set; } = "TXT";

    /// <summary>
    /// The description of the remote site
    /// </summary>
    [Length( 2, 255 )]
    [Column( TypeName = "nvarchar(255)" )]
    public string Description { get; set; } = "Text";

    /// <summary>
    /// A list of remote sites dependent on this file type
    /// </summary>
    public ICollection<RemoteSite> RemoteSites { get; set; } = [];
}

