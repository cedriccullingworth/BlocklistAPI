using BlocklistAPI.Classes;
using BlocklistAPI.Context;

using Microsoft.AspNetCore.Mvc;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
[ApiController]
[Route( "[controller]/devices" )]

public class DevicesController( /* BlocklistDbContext context */ ) : Controller
{
    // private readonly BlocklistDbContext _context = context ?? new BlocklistDbContext( );

    /// <summary>
    /// Fetch details of a device by its MAC address
    /// </summary>
    /// <param name="macAddress">The MAC address of the device</param>
    /// <returns>A device entry if one exists</returns>
    // GET: Devices/Details/MACAddress
    [HttpGet]
    [Route( "/[controller]/[action]/{macAddress}" )]
    public IActionResult Details( string? macAddress )
    {
        if ( macAddress == null )
        {
            return this.BadRequest( "Device details: macAddress is required" ); //this.NotFound( );
        }

        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            return this.Ok( context.GetDevice( macAddress ) );
        }
    }

    /// <summary>
    /// Register a new device
    /// </summary>
    /// <param name="macAddress">The device's MAC address</param>
    /// <returns>The newly added entry if successful</returns>
    [HttpPost]
    [Route( "/[controller]/[action]/{macAddress}" )]
    public IActionResult AddDevice( string? macAddress )
    {
        if ( macAddress == null )
        {
            return this.BadRequest( "Device AddDevice: The device's MAC address is required" );
        }
        else if ( !Subqueries.MACAddressIsValid( macAddress ) )
        {
            return this.BadRequest( "Device AddDevice: The device's MAC address must consist of 6 pairs of hexadecimal characters separated with colons (':')" );
        }

        macAddress = macAddress!.ToUpper( );
        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            // Unneccesary - context.AddDevice simply returns the existing device if it already exists
            //if ( context.Devices.Any( a => a.MACAddress == macAddress ) )
            //    return this.BadRequest( $"The device with MAC address {macAddress} has already been loaded." );
            //else
            return this.Ok( context.AddDevice( macAddress ) );
        }
    }
}
