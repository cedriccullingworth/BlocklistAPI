using BlocklistAPI.Context;
using BlocklistAPI.Models;

using Microsoft.AspNetCore.Mvc;

using SBS.Utilities;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
[ApiController]
[Route( "[controller]/deviceremotesites" )]
//[Route( "[controller]" )]

public class DeviceRemoteSitesController( /*BlocklistDbContext context*/ ) : Controller
{
    /// <summary>
    /// Return the list of remote sites applicable to the deviceID
    /// </summary>
    /// <param name="deviceID"></param>
    /// <returns></returns>
    [HttpGet]
    [Route( "/[controller]/[action]/{deviceID}" )]
    public IActionResult ListDeviceRemoteSites( int deviceID )
    {

        try
        {
            using ( BlocklistDbContext context = new BlocklistDbContext( ) )
            {
                var remoteSites = context.ListRemoteSites( deviceID, null, true );
                return this.Ok
                    (
                        remoteSites
                    );
            }
        }
        catch ( Exception ex )
        {
            return this.Problem( $"Query failed: {StringUtilities.ExceptionMessage( "ListDeviceRemoteSites", ex )}" );
        }
    }

    /// <summary>
    /// Set the date and time that deviceID last downloaded blocklists from remoteSiteID
    /// </summary>
    /// <param name="deviceID">The requesting device identifier</param>
    /// <param name="remoteSiteID">The ID of the download site</param>
    /// <returns>The revised instance of DeviceRemoteSite</returns>
    [HttpPost]
    [Route( "/[controller]/[action]/{deviceID},{remoteSiteID}" )]
    public IActionResult SetLastDownloaded( int deviceID, int remoteSiteID )
    {
        try
        {
            using ( BlocklistDbContext context = new BlocklistDbContext( ) )
            {
                if ( !context.Devices.Any( d => d.ID == deviceID ) )
                {
                    return this.BadRequest( "deviceID is invalid" );
                }

                if ( !context.RemoteSites.Any( r => r.ID == remoteSiteID ) )
                {
                    return this.BadRequest( "remoteSiteID is Invalid" );
                }

                DateTime timeSet = DateTime.UtcNow;
                DeviceRemoteSite? updated = context.SetDownloadedDateTime( deviceID, remoteSiteID );
                return this.Ok( updated );
            }
        }
        catch ( Exception ex )
        {
            return this.Problem( $"Query failed: {StringUtilities.ExceptionMessage( "SetLastDownloaded", ex )}" );
        }
    }
}
