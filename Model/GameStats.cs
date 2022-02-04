using ThronemastersStatsCounter.Extensions;

namespace ThronemastersStatsCounter.Model
{
    public class GameStats : Dictionary<House, HouseStats>
    {
        public int Id { get; }
        public string Name { get; }

        public GameStats(int id, string name)
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
            var houses = Keys.ToHashSet();
            houses.UnionWith(stats.Keys);
            foreach (var key in houses)
            {
                var houseStats = stats[key];
                if (!ContainsKey(key))
                    this[key] = new HouseStats(houseStats.House, houseStats.Player);
                this[key].Sum(houseStats);
            }
        }

        public void SumPlayers(GameStats stats)
        {
            var players = Values.Select(houseStats => houseStats.Player).ToHashSet();
            players.UnionWith(stats.Values.Select(houseStats => houseStats.Player));
            foreach (var player in players)
            {
                var playerStats = stats.Values
                    .FirstOrDefault(v => v.Player == player);
                if (playerStats == null)
                    continue;
                var thisPlayerStats = Values
                    .FirstOrDefault(v => v.Player == player);
                if (thisPlayerStats == null)
                {
                    thisPlayerStats = new HouseStats(playerStats.House, playerStats.Player);
                    this[playerStats.House] = thisPlayerStats;
                }

                thisPlayerStats.Sum(playerStats);
            }
        }
    }
}
