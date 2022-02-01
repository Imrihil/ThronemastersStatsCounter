using System.Text.RegularExpressions;
using ThronemastersStatsCounter.Model;

namespace ThronemastersStatsCounter
{
    public static class StatsCounter
    {
        public static GameStats Count(Log log)
        {
            var stats = new GameStats(log.GameId, log.GameName)
            {
                { House.NA, new HouseStats(House.NA, "N/A") }
            };

            House? movingHouse = null;
            House? fightingHouse = null;
            foreach (var logEvent in log)
            {
                switch (logEvent.Phase)
                {
                    case GamePhase.PLANNING:
                        stats.EnsureHouseCreated(logEvent.Player, logEvent.House);
                        break;
                    case GamePhase.RAVEN:
                        break;
                    case GamePhase.RAID:
                        stats.CountRaid(logEvent);
                        break;
                    case GamePhase.MARCH:
                        movingHouse = logEvent.House;
                        break;
                    case GamePhase.BATTLE:
                        if (logEvent.MainMessage.Contains("to lead his forces."))
                            fightingHouse = GetFightingHouse(fightingHouse, movingHouse, logEvent);
                        if (logEvent.MainMessage.Contains("lost the battle"))
                            stats.CountBattle(movingHouse ?? House.NA, fightingHouse ?? House.NA, logEvent);
                        break;
                    case GamePhase.POWER:
                        if (logEvent.MainMessage.Contains("New Power Tokens"))
                            stats.CountConsolidatePowerOrder(logEvent);
                        if (logEvent.MainMessage.Contains("Houses consolidate new power"))
                            stats.CountPowerConsolidation(logEvent);
                        break;
                    case GamePhase.WESTEROS:
                        if (logEvent.MainMessage.Contains("Game of Thrones - Houses consolidated new powers"))
                            stats.CountGameOfThrones(logEvent);
                        if (logEvent.MainMessage.Contains("for the Iron Throne track."))
                            stats.CountIronThrone(logEvent);
                        if (logEvent.MainMessage.Contains("for the Fiefdoms track."))
                            stats.CountFiefdoms(logEvent);
                        if (logEvent.MainMessage.Contains("for the King's Court track."))
                            stats.CountKingsCourt(logEvent);
                        if (logEvent.MainMessage.Contains("against the wildlings."))
                            stats.CountWildlings(logEvent);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return stats;
        }

        private static void EnsureHouseCreated(this IDictionary<House, HouseStats> stats, string player, House house)
        {
            if (!stats.ContainsKey(house))
                stats.Add(house, new HouseStats(house, player));
        }

        private static void CountRaid(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            if (logEvent.MainMessage.Contains("raided the Consolidate Power"))
                ++stats[logEvent.House].PowerTokens.Raids;
        }

        private static House? GetFightingHouse(House? fightingHouse, House? movingHouse, LogEvent logEvent) =>
            logEvent.House != movingHouse ?
                logEvent.House :
                fightingHouse;

        public static void CountBattle(this IDictionary<House, HouseStats> stats, House movingHouse, House fightingHouse, LogEvent logEvent)
        {
            var looser = logEvent.House;
            var winner = movingHouse == looser ? fightingHouse : movingHouse;
            stats[winner].Battles.Won++;
            stats[looser].Battles.Lost++;
            var casualtiesMessage = logEvent.Messages.FirstOrDefault(m => m.Contains("Following units were killed: "));
            if (casualtiesMessage == null)
                return;
            var footmenMatch = Regex.Match(casualtiesMessage, @"(\d+) Footm\wn");
            if (footmenMatch.Success)
            {
                var footmen = int.Parse(footmenMatch.Groups[1].Value);
                stats[winner].Kills.Footman += footmen;
                stats[looser].Casualties.Footman += footmen;
            }
            var knightsMatch = Regex.Match(casualtiesMessage, @"(\d+) Knight");
            if (knightsMatch.Success)
            {
                var knights = int.Parse(knightsMatch.Groups[1].Value);
                stats[winner].Kills.Knights += knights;
                stats[looser].Casualties.Knights += knights;
            }
            var shipsMatch = Regex.Match(casualtiesMessage, @"(\d+) Ship");
            if (shipsMatch.Success)
            {
                var ships = int.Parse(shipsMatch.Groups[1].Value);
                stats[winner].Kills.Ships += ships;
                stats[looser].Casualties.Ships += ships;
            }
            var siegeEnginesMatch = Regex.Match(casualtiesMessage, @"(\d+) Siege");
            if (siegeEnginesMatch.Success)
            {
                var siegeEngines = int.Parse(siegeEnginesMatch.Groups[1].Value);
                stats[winner].Kills.SiegeEngines += siegeEngines;
                stats[looser].Casualties.SiegeEngines += siegeEngines;
            }
        }

        private static void CountConsolidatePowerOrder(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var tokens = int.Parse(logEvent.MainMessage[^1].ToString());
            stats[logEvent.House].PowerTokens.ConsolidatePower += tokens;
        }

        private static void CountPowerConsolidation(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var matches = Regex.Matches(logEvent.Messages[1], @"(\w+)\(\+(\d+)\)");
            foreach (Match match in matches)
            {
                Enum.TryParse<House>(match.Groups[1].Value, out var house);
                var tokens = int.Parse(match.Groups[2].Value);
                stats[house].PowerTokens.ConsolidatePower += tokens;
            }
        }

        private static void CountGameOfThrones(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var matches = Regex.Matches(logEvent.Messages[1], @"(\w+)\(\+(\d+)\)");
            foreach (Match match in matches)
            {
                Enum.TryParse<House>(match.Groups[1].Value, out var house);
                var tokens = int.Parse(match.Groups[2].Value);
                stats[house].PowerTokens.GameOfThrones += tokens;
            }
        }

        public static void CountIronThrone(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var match = Regex.Match(logEvent.MainMessage, @"\[(\d+)\]");
            var tokens = int.Parse(match.Groups[1].Value);
            stats[logEvent.House].Bids.IronThrone += tokens;
        }

        public static void CountFiefdoms(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var match = Regex.Match(logEvent.MainMessage, @"\[(\d+)\]");
            var tokens = int.Parse(match.Groups[1].Value);
            stats[logEvent.House].Bids.Fiefdoms += tokens;
        }

        public static void CountKingsCourt(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var match = Regex.Match(logEvent.MainMessage, @"\[(\d+)\]");
            var tokens = int.Parse(match.Groups[1].Value);
            stats[logEvent.House].Bids.KingsCourt += tokens;
        }

        public static void CountWildlings(this IDictionary<House, HouseStats> stats, LogEvent logEvent)
        {
            var match = Regex.Match(logEvent.MainMessage, @"\[(\d+)\]");
            var tokens = int.Parse(match.Groups[1].Value);
            stats[logEvent.House].Bids.Wildlings += tokens;
        }
    }
}
