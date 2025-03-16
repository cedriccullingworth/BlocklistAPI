using System.Text.RegularExpressions;

using BlocklistAPI.Context;

namespace BlocklistAPI.Classes;

internal static class Subqueries
{
    internal static DateTime GetLastDownloadDateTime( int remoteSiteID )
    {
        using ( BlocklistDbContext context = new( ) )
        {
            return context.DeviceRemoteSites
                          .Where( r => r.RemoteSiteID == remoteSiteID )
                          .Select( s => s.LastDownloaded )
                          .Max( );
        }
    }

    private const string _macRegex = "^([0-9A-F]{2}[:]){5}([0-9A-F]{2})$";

    internal static bool MACAddressIsValid( string macAddress ) => Regex.Match( macAddress.ToUpper( ), _macRegex ).Success;
    //internal static bool MACAddressIsValid( string macAddress ) => macAddress.Length == 17 && macAddress[ 2 ] == ':' && macAddress[ 5 ] == ':' && macAddress[ 8 ] == ':' && macAddress[ 11 ] == ':' && macAddress[ 14 ] == ':';
}
