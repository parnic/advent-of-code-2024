using System.Net;

namespace aoc2024.Util;

internal static class RetrieveInput
{
    internal static async Task Get(string session, string year, int day)
    {
        var cookies = new CookieContainer();
        cookies.Add(new Uri("https://adventofcode.com"), new Cookie("session", session));

        using var handler = new HttpClientHandler() { CookieContainer = cookies };
        using var client = new HttpClient(handler);

        var inputContents = await client.GetStringAsync($"https://adventofcode.com/{year}/day/{day}/input");
        Directory.CreateDirectory("inputs");
        File.WriteAllText(Path.Combine("inputs", $"{day.ToString().PadLeft(2, '0')}.txt"), inputContents);
    }
}
