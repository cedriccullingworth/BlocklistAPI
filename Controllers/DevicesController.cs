using BlocklistAPI.Context;

using Microsoft.AspNetCore.Mvc;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
/// <param name="context">The name of the context</param>
[ApiController]
[Route( "[controller]/devices" )]

public class DevicesController( /* OpenCartDbContext context */ ) : Controller
{
    // private readonly OpenCartDbContext _context = context ?? new OpenCartDbContext( );

    // GET: Devices/Details/MACAddress
    [HttpGet]
    [Route( "/[controller]/[action]/{macAddress}" )]
    public async Task<IActionResult> Details( string? macAddress )
    {
        if ( macAddress == null )
        {
            return NotFound( );
        }

        using ( OpenCartDbContext context = new OpenCartDbContext( ) )
        {
            return Ok( context.GetDevice( macAddress ) );
        }
    }
}
