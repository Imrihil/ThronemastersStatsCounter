using System.Text.RegularExpressions;
using ThronemastersStatsCounter.Extensions;
using ThronemastersStatsCounter.Model;

namespace ThronemastersStatsCounter
{
    public static class LogParser
    {
        public static Log? Parse(string[] document)
        {
            var log = GetLog(document.First());
            if (log == null)
                return null;

            LogEvent? lastEvent = null;
            foreach (var line in document)
            {
                var isMainEvent = line.StartsWithNumber();
                var isDetail = line.StartsWith("-") || line.StartsWith("[Attack]");

                if (!isMainEvent && !isDetail)
                    continue;

                var input = line.Split("\t");

                if (isMainEvent)
                {
                    lastEvent = HandleMainEvent(input, lastEvent, log);
                }
                else if (isDetail && lastEvent != null)
                {
                    lastEvent = HandleDetail(input, lastEvent, log);
                }
            }

            return log;
        }

        private static LogEvent? HandleDetail(IReadOnlyList<string> input, LogEvent lastEvent, Log log)
        {
            var message = input[0];
            lastEvent.Messages.Add(message);
            if (input.Count < 2)
                return lastEvent;

            var dateTime = DateTime.Parse(input[1]);
            lastEvent.DateTime = dateTime;
            log.Add(lastEvent);
            return null;
        }

        private static LogEvent? HandleMainEvent(IReadOnlyList<string> input, LogEvent? lastEvent, Log log)
        {
            var id = int.Parse(input[0]);
            var turn = int.Parse(input[1]);
            Enum.TryParse<GamePhase>(input[2], out var phase);
            var player = input[3];
            var message = input[4];
            if (input.Count <= 5)
                return new LogEvent(id, turn, phase, player, message);

            var dateTime = DateTime.Parse(input[5]);
            var logEvent = new LogEvent(id, turn, phase, player, message, dateTime);
            log.Add(logEvent);
            return null;

        }

        private static Log? GetLog(string firstLine)
        {
            var match = Regex.Match(firstLine, @"Events of Game (\d+): ""(.+)""");
            if (!match.Success)
                return null;

            var id = int.Parse(match.Groups[1].Value);
            var name = match.Groups[2].Value;

            var log = new Log(id, name);
            return log;
        }
    }
}
