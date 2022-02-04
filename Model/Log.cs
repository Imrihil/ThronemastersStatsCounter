namespace ThronemastersStatsCounter.Model
{
    public class Log : List<LogEvent>
    {
        public int GameId { get; }
        public string GameName { get; }

        public Log(int gameId, string gameName)
        {
            GameId = gameId;
            GameName = gameName;
        }
    }
}
