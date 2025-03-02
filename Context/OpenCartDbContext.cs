using System.Diagnostics;
using System.Reflection;

using BlocklistAPI.Models;

using Microsoft.EntityFrameworkCore;

using SBS.Utilities;

// using SBS.Utilities;

namespace BlocklistAPI.Context;

/// <summary>
/// Using the OpenCart Db to house the tables used by the Blocklist API
/// </summary>
public class OpenCartDbContext : DbContext
{
    private string _connectionString = string.Empty;

    /// <summary>
    /// Constructor
    /// </summary>
    public OpenCartDbContext( ) : base( )
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
    public OpenCartDbContext( DbContextOptions<OpenCartDbContext> options ) : base( options )
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
            optionsBuilder.UseMySQL( this._connectionString ) //, ServerVersion.AutoDetect( _connectionString ) );
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
    /// Extract file types as a list
    /// </summary>
    /// <returns></returns>
    internal List<FileType> ListFileTypes( ) => [ .. this.FileTypes.OrderBy( o => o.Name ) ];

    /// <summary>
    /// Get the date and time of the most recent download from a remote site
    /// </summary>
    /// <param name="deviceID">The device we are doing the check for</param>
    /// <param name="remoteSiteID">The remote site we are doing the check for</param>
    /// <returns></returns>
    internal DateTime? LastDownloaded( int deviceID, int? remoteSiteID ) =>
        this.DeviceRemoteSites.Where(
                                        f => f.Device.ID == deviceID
                                        && ( remoteSiteID == null || f.RemoteSite.ID == remoteSiteID )
                                    )
                              .Select( s => s.LastDownloaded )
                              .Max( );

    /// <summary>
    /// Extract remote sites as a list
    /// </summary>
    /// <param name="remoteSiteID">A remote site ID if only fetching one site</param>
    /// <param name="showAll">If true, list all sites, including those that have been processed recently, otherwise only those whic weren't downloaded in the past 30 minutes</param>
    /// <returns>A list of blocklist download sites</returns>
    //internal List<RemoteSite> ListRemoteSites( int? remoteSiteID, bool showAll = false )
    //{
    //    List<RemoteSite> remoteSites = [];
    //    try
    //    {
    //        OpenCartDbContext context = this;
    //        remoteSites = this.RemoteSites
    //                                .Where( w => w.Active || showAll )
    //                                .Where( w => remoteSiteID == null || w.id == remoteSiteID )
    //                                .Where( w => showAll
    //                                              || (
    //                                                    w.MinimumIntervalMinutes == 0
    //                                                 || w.LastDownloaded.AddMinutes( w.MinimumIntervalMinutes ) < DateTime.UtcNow
    //                                                 )
    //                                      )
    //                                //.Include( i => i.FileType )
    //                                .Select( r => new RemoteSite( )
    //                                {
    //                                    id = r.id,
    //                                    Name = r.Name,
    //                                    /* = this.DeviceRemoteSites
    //                                                         .Where( w => w.RemoteSiteID == r.id )
    //                                                         .Max( m => m.LastDownloaded ),*/
    //                                    //this.LastDownloaded( null, remoteSiteID ),
    //                                    SiteUrl = r.SiteUrl,
    //                                    FileUrls = r.FileUrls,
    //                                    FileTypeID = r.FileTypeID,
    //                                    FileType = this.FileTypes.Where( f => f.id == r.FileTypeID ).Select( s => new FileType( ) { id = s.id, Name = s.Name, Description = s.Description } ).FirstOrDefault( ),
    //                                    // new FileType( )
    //                                    //{
    //                                    //    id = r.FileType.id,
    //                                    //    Name = r.FileType.Name,
    //                                    //    Description = r.FileType.Description,
    //                                    //},
    //                                    Active = r.Active,
    //                                    MinimumIntervalMinutes = r.MinimumIntervalMinutes,
    //                                } )
    //                                .OrderBy( o => o.Name )
    //                                .ToList( );

    //        //var source = from r in remoteSites
    //        //             join f in this.FileTypes on r.FileTypeID equals f.id
    //        //             where ( r.Active || showAll )
    //        //             where ( remoteSiteID is null || r.id == remoteSiteID )
    //        //             //join drs in this.DeviceRemoteSites.Where( w => w.DeviceID == deviceID ) on r equals drs.RemoteSite
    //        //select new RemoteSite( )
    //        //{
    //        //    id = r.id,
    //        //    Name = r.Name,
    //        //    /* = this.DeviceRemoteSites
    //        //                         .Where( w => w.RemoteSiteID == r.id )
    //        //                         .Max( m => m.LastDownloaded ),*/
    //        //    LastDownloaded = r.LastDownloaded,
    //        //    SiteUrl = r.SiteUrl,
    //        //    FileUrls = r.FileUrls,
    //        //    FileTypeID = r.FileTypeID,
    //        //    FileType = new FileType( )
    //        //    {
    //        //        id = f.id,
    //        //        Name = f.Name,
    //        //        Description = f.Description,
    //        //    },
    //        //    Active = r.Active,
    //        //    MinimumIntervalMinutes = r.MinimumIntervalMinutes,
    //        //};

    //        //if ( !showAll )
    //        //    source = source.Where( w => w.MinimumIntervalMinutes == 0
    //        //                                      || w.LastDownloaded.AddMinutes( w.MinimumIntervalMinutes ) < DateTime.UtcNow );
    //    }
    //    catch ( Exception ex )
    //    {
    //        Console.WriteLine( StringUtilities.ExceptionMessage( "ListRemoteSites", ex ) );
    //    }

    //    return remoteSites; // [ .. source.OrderBy( o => o.Name ) ];
    //}

    /// <summary>
    /// NEW: Exclude sites processed less than half and hour ago from the list
    /// </summary>
    /// <param name="deviceID">The identity of the device we're fetching for</param>
    /// <param name="remoteSiteID">A remote site ID if only fetching one site</param>
    /// <param name="showAll">If true, liust all sites, including those that have been processed recently, otherwise only those whic weren't downloaded in the past 30 minutes</param>
    /// <returns>A list of blocklist download sites</returns>
    internal List<RemoteSite> ListRemoteSites( int deviceID, int? remoteSiteID, bool showAll = false )
    {
        // Finally returning suitable results
        // .ToList( ) used in all parts to avoid MySQL DbReader still open exception
        try
        {
            // First, identify only the remote sites that are associated with the device, with that last download date and time
            var drs = this.DeviceRemoteSites
                                            .Where( w => w.Device.ID == deviceID )
                                            .Select( s => new { s.RemoteSiteID, s.LastDownloaded } )
                                            .Distinct( )
                                            .ToList( ); // Somehow this query was returning 2X as many entries as there were in the table. This is why it's treated as a distinct list

            var sitesBase = this.RemoteSites
                         .Include( i => i.FileType )
                         .Where( w => remoteSiteID == null || w.ID == remoteSiteID )
                         .Where( w => w.Active || showAll )
                         .Where( w => showAll
                                       || (
                                           w.MinimumIntervalMinutes == 0
                                           || w.LastDownloaded.AddMinutes( w.MinimumIntervalMinutes ) < DateTime.UtcNow
                                           )
                               )
                         .ToList( );

            return
                [ .. (
                    from r in sitesBase
                    join d in drs on r.ID equals d.RemoteSiteID into drsGroup
                    from d in drsGroup.DefaultIfEmpty( )
                    select new RemoteSite( )
                    {
                        ID = r.ID,
                        Name = r.Name,
                        SiteUrl = r.SiteUrl,
                        FileUrls = r.FileUrls,
                        Active = r.Active,
                        LastDownloaded = d?.LastDownloaded == null ? new DateTime( 2001, 1, 1, 0, 0, 0, 1, 0 ) : d.LastDownloaded,
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
            .OrderBy( o => o.Name ) ];
        }
        catch ( Exception ex )
        {
            Console.WriteLine( StringUtilities.ExceptionMessage( "ListRemoteSites", ex ) );
            return [];
        }
    }

    /// <summary>
    /// Mark the date and time of a download from a remote site for a device
    /// </summary>
    /// <param name="deviceID"></param>
    /// <param name="remoteSiteID"></param>
    internal DeviceRemoteSite? SetDownloadedDateTime( int deviceID, int remoteSiteID )
    {
        DeviceRemoteSite? target = this.DeviceRemoteSites.FirstOrDefault( f => f.Device.ID == deviceID && f.RemoteSite.ID == remoteSiteID );
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
                int id = target!.ID;
                string sqlQuery = $"UPDATE `DeviceRemoteSite` SET `LastDownloaded` = UTC_TIMESTAMP() WHERE `ID` = {id};"; //  DeviceID = {deviceID} AND RemoteSiteID = {remoteSiteID}; ";
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

    internal DbSet<RemoteSite> RemoteSites { get; set; }

    internal DbSet<FileType> FileTypes { get; set; }

    internal DbSet<Device> Devices { get; set; }

    internal DbSet<DeviceRemoteSite> DeviceRemoteSites { get; set; }

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

    internal Device? GetDevice( string macAddress )
    {
        Device? device = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );
        if ( device == null )
        {
            device = new Device( )
            {
                MACAddress = macAddress
            };
            this.Devices.Add( device );
            this.SaveChanges( );
            device = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );
        }

        return device;
    }

    internal static FileType? GetFileType( OpenCartDbContext context, int filetypeID )
    {
        return context.FileTypes
               .Select( s => new FileType( ) { ID = s.ID, Name = s.Name, Description = s.Description } )
               .FirstOrDefault( f => f.ID == filetypeID );//if ( fileType == null )//{//    fileType = new Device( )//    {//        MACAddress = macAddress//    };//    this.Devices.Add( fileType );//    this.SaveChanges( );//    fileType = this.Devices.FirstOrDefault( f => f.MACAddress == macAddress );//}//return fileType;
    }

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
