namespace ThronemastersStatsCounter.Model
{
    public class Log : List<LogEvent>
    {
        public string GameId { get; }
        public string GameName { get; }

        public Log(string gameId, string gameName)
        {
            GameId = gameId;
            GameName = gameName;
        }
    }
}
