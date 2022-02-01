using ThronemastersStatsCounter.Extensions;

namespace ThronemastersStatsCounter.Model
{
    public class GameStats : Dictionary<House, HouseStats>
    {
        public string Id { get; }
        public string Name { get; }

        public GameStats(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => ToString(null);

        public string ToString(string? separator)
        {
            var widths = HouseStats.Headers.Select(s => s.Length).ToArray();
            foreach (var houseStats in Values)
            {
                widths[0] = Math.Max(widths[0], houseStats.House.ToString().Length);
                widths[1] = Math.Max(widths[1], houseStats.Player.Length);
                widths[2] = Math.Max(widths[2], houseStats.Battles.ToString().Length);
                widths[3] = Math.Max(widths[3], houseStats.Kills.ToString().Length);
                widths[4] = Math.Max(widths[4], houseStats.Casualties.ToString().Length);
                widths[5] = Math.Max(widths[5], houseStats.PowerTokens.ToString().Length);
                widths[6] = Math.Max(widths[6], houseStats.Bids.ToString().Length);
            }

            var i = 0;
            var headerLine = "|" +
                             string.Join("|", HouseStats.Headers.Select(header => header.Stretch(widths[i++]))) +
                             "|";
            var separatorLine = "|" + string.Join("+", widths.Select(w => new string('-', w))) + "|";
            var statsLine = string.Join(Environment.NewLine, this.OrderBy(kv => kv.Key).Select(kv => kv.Value.ToString(widths)));

            return $"Game \"{Name}\" [{Id}]:" + Environment.NewLine +
                   headerLine + Environment.NewLine +
                   separatorLine + Environment.NewLine +
                   statsLine;
        }

        public void SumHouses(GameStats stats)
        {
            foreach (var key in Keys)
                this[key].Sum(stats[key]);
        }

        public void SumPlayers(GameStats stats)
        {
            foreach (var value in Values)
            {
                var houseStats = stats.Values
                    .FirstOrDefault(v => v.Player == value.Player);
                if (houseStats == null)
                    continue;

                value.Sum(houseStats);
            }
        }
    }
}
