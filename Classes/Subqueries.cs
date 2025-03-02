using BlocklistAPI.Context;

namespace BlocklistAPI.Classes;

internal static class Subqueries
{
    internal static DateTime GetLastDownloadDateTime( int remoteSiteID )
    {
        using ( OpenCartDbContext context = new( ) )
        {
            return context.DeviceRemoteSites
                          .Where( r => r.RemoteSite.ID == remoteSiteID )
                          .Select( s => s.LastDownloaded )
                          .Max( );
        }
    }
}
