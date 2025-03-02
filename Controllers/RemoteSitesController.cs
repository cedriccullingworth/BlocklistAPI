using BlocklistAPI.Context;
using BlocklistAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Controllers;

/* MAC Address for tests: 2C:3B:70:0C:DA:F5 */

/// <summary>
/// Constructor
/// </summary>
/// <param name="context">The name of the context</param>
[ApiController]
[Route( "[controller]/remotesites" )]
//[Route( "[controller]" )]

public class RemoteSitesController( /* OpenCartDbContext context */ ) : Controller
{
    // private readonly OpenCartDbContext _context = context ?? new OpenCartDbContext( );

    //private readonly ILogger<RemoteSitesController> _logger;

    //public RemoteSitesController( ILogger<RemoteSitesController> logger )
    //{
    //    _logger = logger;
    //}

    // GET: RemoteSites
    /// <summary>
    /// Extract remote sites as a list
    /// </summary>
    /// <param name="deviceID">The ID of the device we're fetching for</param>
    /// <param name="remoteSiteID">A remote site ID if only fetching one site</param>
    /// <param name="showAll">If true, list all sites including those that have been processed recently, otherwise only those which weren't downloaded in the past 30 minutes</param>
    /// <returns>A list of blocklist download sites</returns>
    [HttpGet] //( Name = "RemoteSitesIndex" )]
    [Route( "/[controller]/[action]" )]
    public async Task<IActionResult> ListRemoteSites( int deviceID, int? remoteSiteID, bool showAll = false )
    {
        using ( OpenCartDbContext context = new OpenCartDbContext( ) )
        {

            Device? device = context.Devices.Find( deviceID );
            if ( device is not null )
            {
                return Ok/*View*/
                    (
                        context.ListRemoteSites( deviceID, remoteSiteID, showAll )
                    // Finally returning suitable results
                    // First, identify only the remote sites that are associated with the device, with that last download date and time
                    //source.OrderBy( o => o.Name )
                    //      .ToListAsync( )
                    //_context.RemoteSites.ToList( )
                    );
            }
            else
            {
                if ( deviceID == 0 )
                    return Problem( "Please enter a valid deviceID. deviceID is required." );
                else
                    return Problem( "Unable to find data using the deviceID and/or remoteSiteID provided" );
            }
        }
    }

    // GET: RemoteSites/Details/5
    /// <summary>
    /// Extract details of a remote site
    /// </summary>
    /// <param name="id">The ID of the remote site to fetch details of</param>
    /// <returns>The details of the remote site</returns>
    [HttpGet] //( Name = "RemoteSitesViewDetails" )]
    [Route( "/[controller]/[action]/{id}" )]
    public async Task<IActionResult> Details( int id )
    {
        if ( !RemoteSiteExists( id ) )
        {
            return Problem( "Unable to find any site matching the ID provided" );
        }

        using ( OpenCartDbContext context = new OpenCartDbContext( ) )
        {
            var remoteSite = await context.RemoteSites
            .Include( r => r.FileType )
            .FirstOrDefaultAsync( m => m.ID == id );
            if ( remoteSite == null )
            {
                return NotFound( );
            }

            return Ok/*View*/( remoteSite );
        }
    }

    // GET: RemoteSites/Create
    //[HttpGet] //( Name = "RemoteSitesCreateView" )]
    //[Route( "/[controller]/[action]" )]
    //public IActionResult Create( )
    //{
    //    ViewData[ "FileTypeID" ] = new SelectList( _context.FileTypes, "ID", "ID" );
    //    return Ok/*View*/( );
    //}

    // POST: RemoteSites/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost] //( Name = "RemoteSitesCreate" )]
    //[Route( "/[controller]/[action]" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create( [Bind( "ID,Name,LastDownloaded,SiteUrl,FileUrls,FileTypeID,Active,MinimumIntervalMinutes" )] RemoteSite remoteSite )
    //{
    //    if ( ModelState.IsValid )
    //    {
    //        _context.Add( remoteSite );
    //        await _context.SaveChangesAsync( );
    //        return RedirectToAction( nameof( ListRemoteSites ) );
    //    }
    //    ViewData[ "FileTypeID" ] = new SelectList( _context.FileTypes, "ID", "ID", remoteSite.FileTypeID );
    //    return Ok/*View*/( remoteSite );
    //}

    // GET: RemoteSites/Edit/5
    //[HttpGet] //, ActionName( "RemoteSitesEditView" )]
    //[Route( "/[controller]/[action]" )]
    //public async Task<IActionResult> Edit( int? id )
    //{
    //    if ( id == null )
    //    {
    //        return NotFound( );
    //    }

    //    var remoteSite = await _context.RemoteSites.FindAsync( id );
    //    if ( remoteSite == null )
    //    {
    //        return NotFound( );
    //    }
    //    ViewData[ "FileTypeID" ] = new SelectList( _context.FileTypes, "ID", "ID", remoteSite.FileTypeID );
    //    return Ok/*View*/( remoteSite );
    //}

    //// POST: RemoteSites/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost] //( Name = "RemoteSitesEdit" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit( int id, [Bind( "ID,Name,LastDownloaded,SiteUrl,FileUrls,FileTypeID,Active,MinimumIntervalMinutes" )] RemoteSite remoteSite )
    //{
    //    if ( id != remoteSite.ID )
    //    {
    //        return NotFound( );
    //    }

    //    if ( ModelState.IsValid )
    //    {
    //        try
    //        {
    //            _context.Update( remoteSite );
    //            await _context.SaveChangesAsync( );
    //        }
    //        catch ( DbUpdateConcurrencyException )
    //        {
    //            if ( !RemoteSiteExists( remoteSite.ID ) )
    //            {
    //                return NotFound( );
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction( nameof( ListRemoteSites ) );
    //    }
    //    ViewData[ "FileTypeID" ] = new SelectList( _context.FileTypes, "ID", "ID", remoteSite.FileTypeID );
    //    return Ok/*View*/( remoteSite );
    //}

    //// GET: RemoteSites/Delete/5
    //[HttpGet] //, ActionName( "RemoteSitesViewDelete" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //public async Task<IActionResult> Delete( int? id )
    //{
    //    if ( id == null )
    //    {
    //        return NotFound( );
    //    }

    //    var remoteSite = await _context.RemoteSites
    //        .Include( r => r.FileType )
    //        .FirstOrDefaultAsync( m => m.ID == id );
    //    if ( remoteSite == null )
    //    {
    //        return NotFound( );
    //    }

    //    return Ok/*View*/( remoteSite );
    //}

    //// POST: RemoteSites/Delete/5
    //[HttpPost] //, ActionName( "RemoteSitesDelete" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed( int id )
    //{
    //    var remoteSite = await _context.RemoteSites.FindAsync( id );
    //    if ( remoteSite != null )
    //    {
    //        _context.RemoteSites.Remove( remoteSite );
    //    }

    //    await _context.SaveChangesAsync( );
    //    return RedirectToAction( nameof( ListRemoteSites ) );
    //}

    private bool RemoteSiteExists( int id )
    {
        using ( OpenCartDbContext context = new OpenCartDbContext( ) )
        {
            return context.RemoteSites
                          .Any( e => e.ID == id );
        }
    }
}
