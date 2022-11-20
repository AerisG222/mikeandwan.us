namespace MawMvcApp.ViewModels.Tools.Bandwidth;

public class BandwidthSizeInfo
{
    public static readonly BandwidthSizeInfo OC192 = new("OC192", "9,400 Mbps", 9400000000);
    public static readonly BandwidthSizeInfo OC48 = new("OC48", "2,400 Mbps", 2400000000);
    public static readonly BandwidthSizeInfo GigE = new("GigE", "1000 Mbps", 1000000000);
    public static readonly BandwidthSizeInfo OC12 = new("OC12", "622 Mbps", 622000000);
    public static readonly BandwidthSizeInfo OC3 = new("OC3", "155 Mbps", 155000000);
    public static readonly BandwidthSizeInfo FastEthernet = new("Fast Ethernet", "100 Mbps", 100000000);
    public static readonly BandwidthSizeInfo T3 = new("T3", "45 Mbps", 45000000);
    public static readonly BandwidthSizeInfo T1 = new("T1", "1.54 Mbps", 1540000);
    public static readonly BandwidthSizeInfo CableDsl512 = new("Cable/DSL", "512 Kbps", 512000);
    public static readonly BandwidthSizeInfo CableDsl256 = new("Cable/DSL", "256 Kbps", 256000);
    public static readonly BandwidthSizeInfo CableDsl128 = new("Cable/DSL", "128 Kbps", 128000);
    public static readonly BandwidthSizeInfo ISDN = new("ISDN", "64 Kbps", 64000);
    public static readonly BandwidthSizeInfo Modem56 = new("Modem", "56.6 Kbps", 56600);
    public static readonly BandwidthSizeInfo Modem33 = new("Modem", "33.6 Kbps", 33600);
    public static readonly BandwidthSizeInfo Modem28 = new("Modem", "28.8 Kbps", 28800);
    public static readonly BandwidthSizeInfo Modem14 = new("Modem", "14.4 Kbps", 14400);

    public static readonly BandwidthSizeInfo[] AllSizes =
    {
            OC192,
            OC48,
            GigE,
            OC12,
            OC3,
            FastEthernet,
            T3,
            T1,
            CableDsl512,
            CableDsl256,
            CableDsl128,
            ISDN,
            Modem56,
            Modem33,
            Modem28,
            Modem14
        };

    public string Name { get; }
    public string Speed { get; }
    public long Bps { get; }

    BandwidthSizeInfo(string name, string speed, long bps)
    {
        Name = name;
        Speed = speed;
        Bps = bps;
    }
}
