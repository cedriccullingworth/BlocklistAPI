using BlocklistAPI.Context;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlocklistAPI.Controllers;

/// <summary>
/// Constructor
/// </summary>
/// <param name="context">The name of the context</param>
[ApiController]
[Route( "[controller]/filetypes" )]

public class FileTypesController( /* BlocklistDbContext context */ ) : Controller
{
    // private readonly BlocklistDbContext _context = context ?? new BlocklistDbContext( );

    //private readonly ILogger<FileTypesController> _logger;

    //public FileTypesController( ILogger<FileTypesController> logger )
    //{
    //    _logger = logger;
    //}

    // GET: FileTypes
    [HttpGet] //( Name = "FileTypesIndex" )]
    [Route( "/[controller]/[action]" )]
    public async Task<IActionResult> Index( )
    {
        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            return Ok(/*View
            (*/
                await context.FileTypes
                .OrderBy( o => o.Name )
                .ToListAsync( )
            //);
            );
        }
    }

    /// <summary>
    /// Extract file types as a list
    /// </summary>
    /// <param name="id">The ID of the item to fetch</param>
    /// <returns>The file type details matching the ID</returns>
    // GET: FileTypes/Details/5
    [HttpGet] //( Name = "FileTypesViewDetails" )]
    [Route( "/[controller]/[action]/{id}" )]
    public async Task<IActionResult> Details( int? id )
    {
        if ( id == null )
        {
            return NotFound( );
        }

        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            var fileType = await context.FileTypes
                                                .FirstOrDefaultAsync( m => m.ID == id );
            if ( fileType == null )
            {
                return NotFound( );
            }

            return Ok/*View*/( fileType );
        }
    }

    // GET: FileTypes/Create
    //[HttpGet] //( Name = "CreateView" )]
    //[Route( "/[controller]/[action]" )]
    //public IActionResult Create( )
    //{
    //    return Ok/*View*/( );
    //}

    // POST: FileTypes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost] //( Name = "FileTypesCreate" )]
    //[Route( "/[controller]/[action]" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create( [Bind( "ID,Name,Description" )] FileType fileType )
    //{
    //    if ( ModelState.IsValid )
    //    {
    //        _context.Add( fileType );
    //        await _context.SaveChangesAsync( );
    //        return RedirectToAction( nameof( ListRemoteSites ) );
    //    }

    //    return Ok/*View*/( fileType );
    //}

    // GET: FileTypes/Edit/5
    //[HttpGet] //( Name = "FileTypesEditView" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //public async Task<IActionResult> Edit( int? id )
    //{
    //    if ( id == null )
    //    {
    //        return NotFound( );
    //    }

    //    var fileType = await _context.FileTypes.FindAsync( id );
    //    if ( fileType == null )
    //    {
    //        return NotFound( );
    //    }
    //    return Ok/*View*/( fileType );
    //}

    // POST: FileTypes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost] //( Name = "FileTypesEdit" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit( int id, [Bind( "ID,Name,Description" )] FileType fileType )
    //{
    //    if ( id != fileType.ID )
    //    {
    //        return NotFound( );
    //    }

    //    if ( ModelState.IsValid )
    //    {
    //        try
    //        {
    //            _context.Update( fileType );
    //            await _context.SaveChangesAsync( );
    //        }
    //        catch ( DbUpdateConcurrencyException )
    //        {
    //            if ( !FileTypeExists( fileType.ID ) )
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

    //    return Ok/*View*/( fileType );
    //}

    // GET: FileTypes/Delete/5
    //[HttpGet] //, ActionName( "FileTypesDeleteView" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //public async Task<IActionResult> Delete( int? id )
    //{
    //    if ( id == null )
    //    {
    //        return NotFound( );
    //    }

    //    var fileType = await _context.FileTypes
    //        .FirstOrDefaultAsync( m => m.ID == id );
    //    if ( fileType == null )
    //    {
    //        return NotFound( );
    //    }

    //    return Ok/*View*/( fileType );
    //}

    //// POST: FileTypes/Delete/5
    //[HttpPost] //, ActionName( "FileTypesDelete" )]
    //[Route( "/[controller]/[action]/{id}" )]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed( int id )
    //{
    //    var fileType = await _context.FileTypes.FindAsync( id );
    //    if ( fileType != null )
    //    {
    //        _context.FileTypes.Remove( fileType );
    //    }

    //    await _context.SaveChangesAsync( );
    //    return RedirectToAction( nameof( ListRemoteSites ) );
    //}

    private bool FileTypeExists( int id )
    {
        using ( BlocklistDbContext context = new BlocklistDbContext( ) )
        {
            return context.FileTypes.Any( e => e.ID == id );
        }
    }
}
