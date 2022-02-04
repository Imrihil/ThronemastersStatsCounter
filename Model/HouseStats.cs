using ThronemastersStatsCounter.Extensions;

namespace ThronemastersStatsCounter.Model
{
    public class HouseStats
    {
        public static readonly string[] Headers =
            { "House", "Player", "Battles", "Kills", "Casualties", "Power Tokens", "Bids" };

        public static readonly string[] ExportHeaders =
            { "House", "Player", "Battles", "Kills", "Casualties", "Power Tokens", "Bids" };

        public House House { get; private set; }
        public string Player { get; private set; }
        public Battles Battles { get; } = new();
        public UnitStats Kills { get; } = new();
        public UnitStats Casualties { get; } = new();
        public PowerTokens PowerTokens { get; } = new();
        public Bids Bids { get; } = new();

        public HouseStats(House house, string player)
        {
            House = house;
            Player = player;
        }

        public string ToString(int[] widths) =>
            $"|{House.Stretch(widths[0])}|" +
            $"{Player.Stretch(widths[1])}|" +
            $"{Battles.Stretch(widths[2])}|" +
            $"{Kills.Stretch(widths[3])}|" +
            $"{Casualties.Stretch(widths[4])}|" +
            $"{PowerTokens.Stretch(widths[5])}|" +
            $"{Bids.Stretch(widths[6])}|";

        public void Sum(HouseStats stats)
        {
            if (House != stats.House) House = House.NA;
            if (Player != stats.Player) Player = "N/A";
            Battles.Sum(stats.Battles);
            Kills.Sum(stats.Kills);
            Casualties.Sum(stats.Casualties);
            PowerTokens.Sum(stats.PowerTokens);
            Bids.Sum(stats.Bids);
        }

        public override string ToString() =>
            $"|{House}|{Player}|{Battles}|{Kills}|{Casualties}|{PowerTokens}|{Bids}|";
    }

    public class Battles
    {
        public static readonly string[] Headers =
            { "Won", "Lost" };

        public int Won { get; set; }
        public int Lost { get; set; }

        public void Sum(Battles battles)
        {
            Won += battles.Won;
            Lost += battles.Lost;
        }

        public override string ToString() =>
            $"W:{Won}, L:{Lost}";
    }

    public class UnitStats
    {
        public static readonly string[] Headers =
            { "Footman", "Knights", "Ships", "Siege Engines" };

        public int Footman { get; set; }
        public int Knights { get; set; }
        public int Ships { get; set; }
        public int SiegeEngines { get; set; }

        public void Sum(UnitStats stats)
        {
            Footman += stats.Footman;
            Knights += stats.Knights;
            Ships += stats.Ships;
            SiegeEngines += stats.SiegeEngines;
        }

        public override string ToString() =>
            $"{Footman}F {Knights}K {Ships}S {SiegeEngines}SE ({Footman + Knights * 2 + Ships + SiegeEngines * 2}MP)";
    }

    public class PowerTokens
    {
        public static readonly string[] Headers =
            { "Consolidate Powers", "Raids", "Game Of Thrones" };

        public int ConsolidatePower { get; set; }
        public int Raids { get; set; }
        public int GameOfThrones { get; set; }

        public void Sum(PowerTokens tokens)
        {
            ConsolidatePower += tokens.ConsolidatePower;
            Raids += tokens.Raids;
            GameOfThrones += tokens.GameOfThrones;
        }

        public override string ToString() =>
            $"CP:{(ConsolidatePower > 0 ? "+" : "")}{ConsolidatePower}, R:{(Raids > 0 ? "+" : "")}{Raids}, GoT:{(GameOfThrones > 0 ? "+" : "")}{GameOfThrones}";
    }

    public class Bids
    {
        public static readonly string[] Headers =
            { "Iron Throne", "Fiefdoms", "King's Court", "Wildlings" };

        public int IronThrone { get; set; }
        public int Fiefdoms { get; set; }
        public int KingsCourt { get; set; }
        public int Wildlings { get; set; }

        public void Sum(Bids bids)
        {
            IronThrone += bids.IronThrone;
            Fiefdoms += bids.Fiefdoms;
            KingsCourt += bids.KingsCourt;
            Wildlings += bids.Wildlings;
        }

        public override string ToString() =>
            $"IT:{IronThrone}, F:{Fiefdoms}, KC:{KingsCourt}, W:{Wildlings}";
    }
}
