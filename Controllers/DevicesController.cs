using BlocklistAPI.Context;

using Microsoft.AspNetCore.Mvc;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
/// <param name="context">The name of the context</param>
[ApiController]
[Route( "[controller]/devices" )]

public class DevicesController( /* BlocklistDbContext context */ ) : Controller
{
    // private readonly BlocklistDbContext _context = context ?? new BlocklistDbContext( );

    // GET: Devices/Details/MACAddress
    [HttpGet]
    [Route( "/[controller]/[action]/{macAddress}" )]
    public async Task<IActionResult> Details( string? macAddress )
    {
        if ( macAddress == null )
        {
            return NotFound( );
        }

        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            return Ok( context.GetDevice( macAddress ) );
        }
    }
}
