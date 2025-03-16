using System.Diagnostics;
using System.Reflection;

using BlocklistAPI.Classes;
using BlocklistAPI.Models;

using Microsoft.EntityFrameworkCore;

using SBS.Utilities;

// using SBS.Utilities;

namespace BlocklistAPI.Context;

/// <summary>
/// Using the OpenCart Db to house the tables used by the Blocklist API
/// </summary>
public class BlocklistDbContext : DbContext
{
    private string _connectionString = string.Empty;

    /// <summary>
    /// Constructor
    /// </summary>
    public BlocklistDbContext( ) : base( )
    {
        this._connectionString = GetConnectionString( );

        try
        {
            this.Database.SetConnectionString( this._connectionString );
            this.Database.SetCommandTimeout( 30 );
        }
        catch ( Exception )
        {
            // Console.WriteLine( StringUtilities.ExceptionMessage( "BlocklistDbContext()", ex ) );
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public BlocklistDbContext( DbContextOptions<BlocklistDbContext> options ) : base( options )
    {
        this._connectionString = GetConnectionString( );

        try
        {
            this.Database.SetConnectionString( this._connectionString );
            this.Database.SetCommandTimeout( 30 );
        }
        catch ( Exception )
        {
            // Console.WriteLine( StringUtilities.ExceptionMessage( "BlocklistDbContext(options)", ex ) );
        }
    }

    /// <summary>
    /// Fetch the connection string from the appsettings.json file
    /// </summary>
    /// <returns></returns>
    internal static string GetConnectionString( )
    {
        // When using efbundle, it looks like the path is empty, so I've changed the code to handle that

        string configFilePath = Assembly.GetExecutingAssembly( ).Location;
        if ( !string.IsNullOrEmpty( configFilePath ) )
        {
            if ( configFilePath.Contains( '\\' ) )
                configFilePath = $"{configFilePath[ 0..configFilePath.LastIndexOf( '\\' ) ]}\\appsettings.json";
            else if ( configFilePath.Contains( '/' ) )
                configFilePath = $"{configFilePath[ 0..configFilePath.LastIndexOf( '/' ) ]}/appsettings.json";
        }
        else
            configFilePath = "appsettings.json";

        IConfigurationBuilder configuration = new ConfigurationBuilder( ).AddJsonFile( configFilePath );
        IConfigurationRoot config = configuration.Build( );
        return config.GetConnectionString( "sbsdomain" ) ?? string.Empty;
    }

    /// <summary>
    /// OnConfiguring override
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
    {
        if ( string.IsNullOrEmpty( this._connectionString ) )
            this._connectionString = GetConnectionString( );

        try
        {
            optionsBuilder.UseSqlServer( this._connectionString ) // .UseMySQL( this._connectionString ) //, ServerVersion.AutoDetect( _connectionString ) );
                          .EnableSensitiveDataLogging( ) // Enable sensitive data logging
                          .LogTo( Console.WriteLine, LogLevel.Information ); // Log SQL queries to the console
            base.OnConfiguring( optionsBuilder );
        }
        catch ( Exception )
        {
            // Console.WriteLine( StringUtilities.ExceptionMessage( "OnConfiguring", ex ) );
        }
    }

    /// <summary>
    /// OnModelCreating override
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<RemoteSite>( )
                    .ToTable( "RemoteSite" )
                    .HasMany<DeviceRemoteSite>( )
                    .WithOne( o => o.RemoteSite );
        //.HasKey( k => k.id );

        modelBuilder.Entity<Device>( )
            .ToTable( "Device" )
            .HasMany<DeviceRemoteSite>( )
            .WithOne( o => o.Device );

        modelBuilder.Entity<DeviceRemoteSite>( )
            .ToTable( "DeviceRemoteSite" )
            .HasOne( o => o.RemoteSite );
        //.WithMany( w => w.DeviceRemoteSites );

        modelBuilder.Entity<DeviceRemoteSite>( )
            .ToTable( "DeviceRemoteSite" )
            .HasOne( o => o.Device )
            .WithMany( w => w.DeviceRemoteSites );

        base.OnModelCreating( modelBuilder );
    }

    //internal void EnsureDataExists( )
    //{
    //    if ( !this.RemoteSites.Any( ) )
    //    {
    //        this.RemoteSites.Add( new RemoteSite( )
    //        {
    //            Name = "Feodo",
    //            SiteUrl = "https://feodotracker.abuse.ch",
    //            FileUrls = "https://feodotracker.abuse.ch/downloads/ipblocklist_recommended.json, https://feodotracker.abuse.ch/downloads/ipblocklist.json",
    //            FileType = this.FileTypes
    //                           .FirstOrDefault( f => f.Name == "JSON" ),
    //            FileTypeID = this.FileTypes
    //                           .First( f => f.Name == "JSON" )
    //                           .id,
    //            LastDownloaded = null,
    //            Active = true
    //        } );

    //        this.SaveChanges( );
    //    }

    //}

    /// <summary>
    /// Adds a device if no record matches the MAC address
    /// </summary>
    /// <param name="macAddress">The MAC address</param>
    /// <returns>The newly added device</returns>
    internal Device? AddDevice( string macAddress )
    {
        Device? device = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );
        if ( device == null )
        {
            if ( Subqueries.MACAddressIsValid( macAddress ) )
            {
                device = new Device( )
                {
                    MACAddress = macAddress,
                };

                try
                {
                    // EF Core was insisting on specifying the ID, which is an IDENTITY value, so replaced SaveChanges with this
                    string sqlQuery = $"INSERT Device ( MACAddress ) VALUES ( '{macAddress}' )";
                    this.Database.ExecuteSqlRaw( sqlQuery );
                    this.Entry<Device>( device ).Reload( );
                }
                catch ( Exception )
                {
                    // Console.WriteLine( StringUtilities.ExceptionMessage( "AddDevice", ex ) );
                }

                device = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );
            }
        }

        return device;
    }

    //private bool MACAddressIsValid( string macAddress ) => macAddress.Length == 17 && macAddress[ 2 ] == ':' && macAddress[ 5 ] == ':' && macAddress[ 8 ] == ':' && macAddress[ 11 ] == ':' && macAddress[ 14 ] == ':';

    // <summary>
    // Extract file types as a list
    // </summary>
    // <returns></returns>
    //    internal List<FileType> ListFileTypes( ) => [ .. this.FileTypes.OrderBy( o => o.Name ) ];

    /// <summary>
    /// Get the date and time of the most recent download from a remote site
    /// </summary>
    /// <param name="deviceID">The device we are doing the check for</param>
    /// <param name="remoteSiteID">The remote site we are doing the check for</param>
    /// <returns></returns>
    internal DateTime? LastDownloaded( int deviceID, int? remoteSiteID ) =>
        this.DeviceRemoteSites.Where(
                                        f => f.DeviceID == deviceID
                                        && ( remoteSiteID == null || f.RemoteSiteID == remoteSiteID )
                                    )
                              .Select( s => s.LastDownloaded )
                              .FirstOrDefault( );

    /// <summary>
    /// NEW: Exclude sites processed less than their MinimumIntervalMinutes ago from the list
    /// This looks like a good candidate for replacment with a stored procedure call
    /// </summary>
    /// <param name="deviceID">The identity of the device we're fetching for</param>
    /// <param name="remoteSiteID">A remote site ID if only fetching one site</param>
    /// <param name="showAll">If true, liust all sites, including those that have been processed recently, otherwise only those whic weren't downloaded in the past 30 minutes</param>
    /// <returns>A list of blocklist download sites</returns>
    internal IEnumerable<RemoteSite>? ListRemoteSites( int deviceID, int? remoteSiteID, bool showAll = false )
    {
        // Finally returning suitable results
        // .ToList( ) used in all parts to avoid DbReader still open exception
        try
        {
            // First, identify only the remote sites that are associated with the device, with that last download date and time
            // Somehow this query was returning 2X as many entries as there were in the table. This is why it's treated as a distinct list
            IQueryable<DeviceRemoteSite> drs = this.DeviceRemoteSites
                                            .Where( w => w.DeviceID == deviceID );
            //                                          .Select( s => new SiteLastDownloaded( s.RemoteSiteID, s.LastDownloaded ) )
            //                                          .ToList( );
            //.Distinct( )
            //.ToList( );

            IQueryable<RemoteSite> sitesBase = this.RemoteSites
                                                   .Include( i => i.FileType )
                                                   .Where( w => remoteSiteID == null || w.ID == remoteSiteID )
                                                   .Where( w => w.Active || showAll )
                                                   .Where( w => showAll
                                                                    ||
                                                                            w.MinimumIntervalMinutes == 0
                                                                         || w.LastDownloaded.AddMinutes( w.MinimumIntervalMinutes ) < DateTime.UtcNow

                                                         );
            //.ToList( );
            //.ToList( );

            if ( drs.Count( ) == 0 )
            {
                drs = this.AddRemoteSiteDeviceLinks( deviceID, sitesBase );
            }

            return
                 (
                    from r in sitesBase
                    join d in drs on r.ID equals d.RemoteSiteID into drsGroup
                    from d in drsGroup.DefaultIfEmpty( ) // We've added any missing links; this shouldn't be necessary
                    select new RemoteSite( )
                    {
                        ID = r.ID,
                        Name = r.Name,
                        SiteUrl = r.SiteUrl,
                        FileUrls = r.FileUrls,
                        Active = r.Active,
                        LastDownloaded = d.LastDownloaded, // == null ? new DateTime( 2001, 1, 1, 0, 0, 0, 1, 0 ) : d.LastDownloaded,
                        FileTypeID = r.FileTypeID,
                        FileType = new FileType( )
                        {
                            ID = r.FileTypeID,
                            Name = r.FileType!.Name,
                            Description = r.FileType.Description
                        },
                        MinimumIntervalMinutes = r.MinimumIntervalMinutes
                    }
                )
            .OrderBy( o => o.Name )
            .ToList( );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( StringUtilities.ExceptionMessage( "ListRemoteSites", ex ) );
            return [];
        }
    }

    /// <summary>
    /// Creates DeviceRemoteSite links where not already linked
    /// </summary>
    /// <param name="deviceID"></param>
    /// <param name="sitesBase"></param>
    /// <returns></returns>
    private IQueryable<DeviceRemoteSite> AddRemoteSiteDeviceLinks( int deviceID, IQueryable<RemoteSite> sitesBase )
    {
        var existing = this.DeviceRemoteSites
                                             .Where( w => w.DeviceID == deviceID )
                                             .Select( s => s.RemoteSiteID )
                                             .OrderBy( o => o );
        // Generate device remote site entries for all remote sites
        foreach ( RemoteSite site in sitesBase.Where( w => !existing.Contains( w.ID ) ) )
        {
            this.DeviceRemoteSites.Add( new DeviceRemoteSite( )
            {
                Device = this.Devices.First( f => f.ID == deviceID ),
                DeviceID = deviceID,
                RemoteSite = site,
                RemoteSiteID = site.ID,
                LastDownloaded = new DateTime( 2001, 1, 1, 0, 0, 0, 1, 0 )
            } );
        }

        this.SaveChanges( );
        return this.DeviceRemoteSites
                   .Where( w => w.DeviceID == deviceID );
        //.Select( s => new SiteLastDownloaded( s.RemoteSiteID, s.LastDownloaded ) )
        //.ToList( );
        //.Distinct( );
        //.ToList( );
        //        return drs;
    }

    /// <summary>
    /// Mark the date and time of a download from a remote site for a device
    /// </summary>
    /// <param name="deviceID"></param>
    /// <param name="remoteSiteID"></param>
    internal DeviceRemoteSite? SetDownloadedDateTime( int deviceID, int remoteSiteID )
    {
        DeviceRemoteSite? target = this.DeviceRemoteSites.FirstOrDefault( f => f.DeviceID == deviceID && f.RemoteSiteID == remoteSiteID );
        //if ( !this.DeviceRemoteSites.Any( a => a.Device.id == deviceID && a.RemoteSite.id == remoteSiteID ) )
        if ( target is null )
        {
            this.DeviceRemoteSites.Add( new DeviceRemoteSite( )
            {
                Device = this.Devices.First( f => f.ID == deviceID ),
                RemoteSite = this.RemoteSites.First( f => f.ID == remoteSiteID ),
                LastDownloaded = DateTime.UtcNow
            } );

            try
            {
                this.SaveChanges( );
                target = this.DeviceRemoteSites
                             .FirstOrDefault( f => f.DeviceID == deviceID && f.RemoteSiteID == remoteSiteID );
            }
            catch ( Exception ex )
            {
                Debug.Print( StringUtilities.ExceptionMessage( "dbContext.SetDownloadedDateTime", ex ) );
            }
        }
        else
        {
            try
            {
                if ( target is null )
                    return null;

                // Performance on this is still not ideal, but regarded as beyond my control (This runs on a remote server against a remote database which I do not control
                int id = target!.ID;
                //string sqlQuery = $"UPDATE `DeviceRemoteSite` SET `LastDownloaded` = UTC_TIMESTAMP() WHERE `ID` = {id};"; //  DeviceID = {deviceID} AND RemoteSiteID = {remoteSiteID}; ";
                string sqlQuery = $"UPDATE DeviceRemoteSite SET LastDownloaded = GETUTCDATE() WHERE ID = {id};"; //  DeviceID = {deviceID} AND RemoteSiteID = {remoteSiteID}; ";
                //sqlQuery += $" SELECT `ID`, `DeviceID`, `RemoteSiteID`, `LastDownloaded` FROM `DeviceRemoteSite` WHERE `ID` = {id};"; // `DeviceID` = {deviceID} AND `RemoteSiteID` = {remoteSiteID};";
                //target = this.DeviceRemoteSites
                //             .FromSqlRaw<DeviceRemoteSite>( sqlQuery, [] )
                //             .ToList( )
                //             .First( );
                this.Database.ExecuteSqlRaw( sqlQuery );
                this.Entry<DeviceRemoteSite>( target ).Reload( );
                return this.DeviceRemoteSites.First( f => f.ID == id ); //.First( f => f.Device.ID == deviceID && f.RemoteSite.ID == remoteSiteID );
            }
            catch ( Exception ex )
            {
                Debug.Print( StringUtilities.ExceptionMessage( "dbContext.SetDownloadedDateTime", ex ) );
            }
        }

        return target;
    }

    //internal DateTime GetLastDownloaded( int deviceID, int? remoteSiteID ) =>
    //    this.DeviceRemoteSites
    //        .FirstOrDefault( f => f.DeviceID == deviceID && ( remoteSiteID == null || f.RemoteSiteID == remoteSiteID ) )
    //        .LastDownloaded ?? new DateTime( 2001, 1, 1 );

    /// <summary>
    /// Represents the data in the RemoteSite table
    /// </summary>
    public DbSet<RemoteSite> RemoteSites { get; set; }

    /// <summary>
    /// Represents the data in the FileType table
    /// </summary>
    public DbSet<FileType> FileTypes { get; set; }

    /// <summary>
    /// Represents the data in the Device table
    /// </summary>
    public DbSet<Device> Devices { get; set; }

    /// <summary>
    /// Represents the data in the DeviceRemoteSite table
    /// </summary>
    public DbSet<DeviceRemoteSite> DeviceRemoteSites { get; set; }

    /// <summary>
    /// Verify that the data need to start exists
    /// </summary>
    /// <returns>True if there are file types and remote sites</returns>
    internal bool ConfirmStartupDataExists( )
    {
        bool dataExists = this.FileTypes.Count( ) >= 9 && this.RemoteSites.Count( ) >= 24;
        if ( !dataExists )
        {
            dataExists = this.LoadFileTypes( );
            if ( dataExists )
            {
                dataExists = this.LoadRemoteSites( );
            }
        }

        return dataExists;
    }

    /// <summary>
    /// Returns the device tuple matching the MAC address
    /// Note: Adds the MACAddress data if no match is found
    /// </summary>
    /// <param name="macAddress">The MAC address</param>
    /// <returns>The matching device tuple</returns>
    internal Device? GetDevice( string macAddress )
    {
        Device? device = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );
        if ( device == null )
        {
            device = this.AddDevice( macAddress );
        }

        return device;
    }

    /// <summary>
    /// Returns the file type with ID matching filetypeID
    /// </summary>
    /// <param name="filetypeID"></param>
    /// <returns>The file type with ID matching filetypeID</returns>
    internal FileType? GetFileType( /*BlocklistDbContext context, */int filetypeID ) => this.FileTypes
               .Select( s => new FileType( ) { ID = s.ID, Name = s.Name, Description = s.Description } )
               .FirstOrDefault( f => f.ID == filetypeID );
    //{
    //    return this.FileTypes
    //           .Select( s => new FileType( ) { ID = s.ID, Name = s.Name, Description = s.Description } )
    //           .FirstOrDefault( f => f.ID == filetypeID );//if ( fileType == null )//{//    fileType = new Device( )//    {//        MACAddress = macAddress//    };//    this.Devices.Add( fileType );//    this.SaveChanges( );//    fileType = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );//}//return fileType;
    //}

    /// <summary>
    /// Add the initial file types if they don't exist
    /// </summary>
    /// <returns>true if successful</returns>
    private bool LoadFileTypes( )
    {
        bool result = this.FileTypes.Count( ) >= 9;
        if ( !result )
        {
            try { this.FileTypes.Add( new FileType( ) { ID = 1, Name = "TXT", Description = "Single column text file listing IP addresses" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 2, Name = "JSON", Description = "Json" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 3, Name = "XML", Description = "XML" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 4, Name = "TAB", Description = "Tab delimited" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 5, Name = "JSONZIP", Description = "Zip archive containing Json" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 6, Name = "TXTZIP", Description = "Zip archive containing text" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 7, Name = "DELIMZIP", Description = "Zip archive containing delimited data" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 8, Name = "TXTALIEN", Description = "AlienVault text layout" } ); } catch { }
            try { this.FileTypes.Add( new FileType( ) { ID = 9, Name = "CSV", Description = "Comma delimited" } ); } catch { }

            try
            {
                //   this.Database.OpenConnection( );
                //   this.Database.ExecuteSqlRaw( "SET IDENTITY_INSERT dbo.FileType ON" );
                this.SaveChanges( );
                //   this.Database.ExecuteSqlRaw( "SET IDENTITY_INSERT dbo.FileType OFF" );
                result = this.FileTypes.Count( ) == 9;
            }
            catch ( DbUpdateException )
            {
                // Console.WriteLine( StringUtilities.ExceptionMessage( "LoadFileTypes", ex1 ) );
            }
            catch ( Exception )
            {
                // Console.WriteLine( StringUtilities.ExceptionMessage( "LoadFileTypes", ex ) );
            }
            finally
            {
                this.Database.CloseConnection( );
            }
        }

        return result;
    }

    /// <summary>
    /// Add the initial remote sites if they don't exist
    /// </summary>
    /// <returns>true if successful</returns>
    private bool LoadRemoteSites( )
    {
        bool result = this.RemoteSites.Count( ) >= 24;
        if ( !result )
        {
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 1, Name = "Feodo", SiteUrl = "https://feodotracker.abuse.ch", FileUrls = "https://feodotracker.abuse.ch/downloads/ipblocklist_recommended.json, https://feodotracker.abuse.ch/downloads/ipblocklist.json", FileTypeID = 2, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 2, Name = "MyIP", SiteUrl = "https://myip.ms", FileUrls = "https://myip.ms/files/blacklist/general/latest_blacklist.txt", FileTypeID = 9, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 3, Name = "FireHOL Level 3", SiteUrl = "https://raw.githubusercontent.com/firehol", FileUrls = "https://raw.githubusercontent.com/firehol/blocklist-ipsets/master/firehol_level3.netset", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 4, Name = "GreenSnow", SiteUrl = "https://greensnow.co/", FileUrls = "https://blocklist.greensnow.co/greensnow.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 5, Name = "AlienVault", SiteUrl = "https://reputation.alienvault.com", FileUrls = "https://reputation.alienvault.com/reputation.generic", FileTypeID = 8, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 6, Name = "Binary Defense Systems Artillery Threat Intelligence Feed and Banlist Feed", SiteUrl = "https://www.binarydefense.com", FileUrls = "https://www.binarydefense.com/banlist.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 7, Name = "CI Army", SiteUrl = "https://cinsscore.com", FileUrls = "https://cinsscore.com/list/ci-badguys.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 8, Name = "dan.me.uk torlist", SiteUrl = "https://www.dan.me.uk", FileUrls = "https://www.dan.me.uk/torlist/index.html", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 45 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 9, Name = "Emerging Threats Compromised and Firewall Block List", SiteUrl = "https://www.emergingthreats.net", FileUrls = "https://rules.emergingthreats.net/blockrules/compromised-ips.txt, https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 10, Name = "Internet Storm Center DShield", SiteUrl = "https://feeds.dshield.org", FileUrls = "https://feeds.dshield.org/block.txt", FileTypeID = 4, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 11, Name = "Internet Storm Center Shodan", SiteUrl = "https://isc.sans.edu", FileUrls = "https://isc.sans.edu/api/threatlist/shodan/shodan.txt", FileTypeID = 3, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 12, Name = "IBM X-Force Exchange", SiteUrl = "https://exchange.xforce.ibmcloud.com/", FileUrls = "https://iplists.firehol.org/files/xforce_bccs.ipset", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 13, Name = "pgl.yoyo.org AdServers", SiteUrl = "https://pgl.yoyo.org", FileUrls = "https://pgl.yoyo.org/adservers/iplist.php?ipformat=&showintro=0&mimetype=plaintext", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 14, Name = "ScriptzTeam", SiteUrl = "https://github.com/scriptzteam/IP-BlockList-v4/blob", FileUrls = "https://raw.githubusercontent.com/scriptzteam/IP-BlockList-v4/refs/heads/main/ips.txt", FileTypeID = 4, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 15, Name = "PAllebone", SiteUrl = "https://github.com/pallebone/StrictBlockPAllebone", FileUrls = "https://raw.githubusercontent.com/pallebone/StrictBlockPAllebone/master/BlockIP.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 16, Name = "Blocklist.de", SiteUrl = "http://lists.blocklist.de/lists/", FileUrls = "http://lists.blocklist.de/lists/all.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 17, Name = "CyberCrime-Tracker", SiteUrl = "https://cybercrime-tracker.net/fuckerz.php", FileUrls = "https://cybercrime-tracker.net/rss.xml", FileTypeID = 3, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 18, Name = "DigitalSide Threat-Intel Repository", SiteUrl = "https://osint.digitalside.it/", FileUrls = "https://osint.digitalside.it/Threat-Intel/lists/latestips.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 19, Name = "abuse.ch", SiteUrl = "https://sslbl.abuse.ch/blacklist/", FileUrls = "https://sslbl.abuse.ch/blacklist/sslipblacklist.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 22, Name = "Miroslav Stampar", SiteUrl = "https://github.com/stamparm", FileUrls = "https://raw.githubusercontent.com/stamparm/ipsum/master/ipsum.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 24, Name = "James Brine", SiteUrl = "https://jamesbrine.com.au", FileUrls = "https://jamesbrine.com.au/csv", FileTypeID = 9, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 25, Name = "NoThink!", SiteUrl = "https://www.nothink.org", FileUrls = "https://www.nothink.org/honeypots/honeypot_ssh_blacklist_2019.txt, https://www.nothink.org/honeypots/honeypot_telnet_blacklist_2019.txt", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 26, Name = "Rutgers Blacklisted IPs", SiteUrl = "https://report.cs.rutgers.edu/mrtg/drop/dropstat.cgi?start=-86400", FileUrls = "https://report.cs.rutgers.edu/DROP/attackers", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try { this.RemoteSites.Add( new RemoteSite( ) { ID = 27, Name = "FireHOL Level 1", SiteUrl = "http://iplists.firehol.org/?ipset=firehol_level1", FileUrls = "https://raw.githubusercontent.com/ktsaou/blocklist-ipsets/master/firehol_level1.netset", FileTypeID = 1, /* LastDownloaded = new DateTime( 2024, 12, 31, 12, 0, 0 ), */ Active = true, MinimumIntervalMinutes = 0 } ); } catch { }
            try
            {
                //this.Database.OpenConnection( );
                //this.Database.ExecuteSqlRaw( "SET IDENTITY_INSERT dbo.RemoteSite ON" );
                this.SaveChanges( );
                //this.Database.ExecuteSqlRaw( "SET IDENTITY_INSERT dbo.RemoteSite OFF" );
                result = this.RemoteSites.Count( ) >= 24;
            }
            catch ( DbUpdateException )
            {
                // Console.WriteLine( StringUtilities.ExceptionMessage( "LoadRemoteSites", ex1 ) );
            }
            catch ( Exception )
            {
                // Console.WriteLine( StringUtilities.ExceptionMessage( "LoadRemoteSites", ex ) );
            }
            finally
            {
                this.Database.CloseConnection( );
            }
        }

        return result;
    }
}
