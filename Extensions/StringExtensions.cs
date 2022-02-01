namespace ThronemastersStatsCounter.Extensions
{
    public static class StringExtensions
    {
        public static bool StartsWithNumber(this string line) =>
            int.TryParse(line.Split().FirstOrDefault() ?? "", out _);

        public static string Stretch(this object? value, int width)
        {
            var length = value?.ToString()?.Length ?? 0;
            return length < width ?
                $"{value}{new string(' ', width - length)}" :
                $"{value}";
        }
    }
}
