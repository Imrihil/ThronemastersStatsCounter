// See https://aka.ms/new-console-template for more information

using FluentAssertions;
using ThronemastersStatsCounter;
using ThronemastersStatsCounter.Model;

const string dataDirectory = "data";

var data = Directory.GetFiles(dataDirectory);
if (data.Length == 1)
{
    var fileName = data[0][5..^4];
    var id = int.Parse(fileName);
    CountStats(id);
    return;
}

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    Console.WriteLine("Application cancelled");
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

while (!cts.IsCancellationRequested)
{
    await Task.Delay(10);
    if (cts.IsCancellationRequested)
        break;

    Console.Write("Type id of your game: ");
    var line = Console.ReadLine();
    if (line == null)
        continue;

    GameStats? housesSumStats = null;
    GameStats? playersSumStats = null;
    var inputs = line.Split(",");
    foreach (var input in inputs)
    {
        if (!int.TryParse(input, out var id) || id <= 0)
        {
            Console.WriteLine("Id must be a positive number! Try again.");
            break;
        }

        var stats = CountStats(id);
        if (stats != null)
        {
            if (housesSumStats == null)
                housesSumStats = stats;
            else
                housesSumStats.SumHouses(stats);
            if (playersSumStats == null)
                playersSumStats = stats;
            else
                playersSumStats.SumPlayers(stats);
        }
    }

    if (inputs.Length <= 1)
        continue;

    Console.WriteLine("Houses report:");
    Console.WriteLine(housesSumStats?.ToString());

    Console.WriteLine("Players report:");
    Console.WriteLine(playersSumStats?.ToString());
}

static GameStats? CountStats(int id)
{
    Console.WriteLine("Loading document...");
    var text = LoadDocument($"{dataDirectory}/{id}.log");
    if (text == null)
    {
        Console.WriteLine($"Logs for the game {id} was not found.");
        return null;
    }

    Console.WriteLine("Document is loaded successfully.");
    Console.WriteLine("Parsing logs...");
    var log = LogParser.Parse(text);
    if (log == null)
        return null;
    for (var i = 0; i < log.Count - 1; ++i)
        log[i].Id.Should().Be(log[i + 1].Id - 1);
    Console.WriteLine("Logs are parsed successfully");
    Console.WriteLine("Counting stats...");
    var stats = StatsCounter.Count(log);
    Console.WriteLine("Stats report:");
    Console.WriteLine(stats.ToString());

    return stats;
}

static string[]? LoadDocument(string path)
{
    try
    {
        return File.ReadAllLines(path);
    }
    catch (FileNotFoundException)
    {
        return null;
    }
}

