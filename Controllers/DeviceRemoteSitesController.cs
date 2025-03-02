using BlocklistAPI.Context;
using BlocklistAPI.Models;

using Microsoft.AspNetCore.Mvc;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
/// <param name="context">The name of the context</param>
[ApiController]
[Route( "[controller]/deviceremotesites" )]
//[Route( "[controller]" )]

public class DeviceRemoteSitesController( BlocklistDbContext context ) : Controller
{
    // GET: RemoteSites
    [HttpGet]
    [Route( "/[controller]/[action]/{deviceID}" )]
    public async Task<IActionResult> ListDeviceRemoteSites( int deviceID )
    {
        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            var remoteSites = context.ListRemoteSites( deviceID, null, true );
            return Ok
                (
                    remoteSites
                );
        }
    }

    [HttpPost]
    [Route( "/[controller]/[action]/{deviceID},{remoteSiteID}" )]
    public async Task<IActionResult> SetLastDownloaded( int deviceID, int remoteSiteID )
    {
        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            if ( !context.Devices.Any( d => d.ID == deviceID ) )
            {
                return Problem( "Invalid deviceID )" );
            }

            if ( !context.RemoteSites.Any( r => r.ID == remoteSiteID ) )
            {
                return Problem( "Invalid remoteSiteID" );
            }

            DateTime timeSet = DateTime.UtcNow;
            DeviceRemoteSite? updated = context.SetDownloadedDateTime( deviceID, remoteSiteID );
            return Ok( updated );
        }
    }
}
