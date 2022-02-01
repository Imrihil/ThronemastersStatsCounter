namespace ThronemastersStatsCounter.Model
{
    public class LogEvent
    {
        public int Id { get; init; }
        public int Turn { get; init; }
        public GamePhase Phase { get; init; }
        public string Player { get; init; }
        public Messages Messages { get; init; }
        public House House => Messages.House;
        public string MainMessage => Messages.Main;
        public DateTime? DateTime { get; set; }

        public LogEvent(int id, int turn, GamePhase phase, string player, string message,
            DateTime? dateTime = null)
        {
            Id = id;
            Turn = turn;
            Phase = phase;
            Player = player;
            Messages = new Messages { message };
            DateTime = dateTime;
        }

        public override string ToString() =>
            $"[{Id}]\t{Turn}\t{Phase}\t{Player}\t{Messages}\t{DateTime}";
    }

    public class Messages : List<string>
    {
        public string Main => this.First();
        public House House => Enum.TryParse<House>(Main.Split()[Main.StartsWith("Wildlings Attack: ") ? 2 : 0], out var house) ? house : House.NA;

        public override string ToString() =>
            string.Join(" ", this);
    }
}
