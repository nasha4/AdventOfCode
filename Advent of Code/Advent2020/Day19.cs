using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public class Day19(bool isPart1) : IAdventPuzzle
{
    private Dictionary<string, string[]> rules = [];

    public string Solve(InputHelper inputHelper)
    {
        rules = inputHelper.EachLineInSection(line => line.Split(": ")).ToDictionary(x => x[0], x => x[1].Split(' '));

        var rule = new Regex($"^{Rule("0")}$", RegexOptions.ExplicitCapture);

        return inputHelper.EachLine().Count(rule.IsMatch).ToString();
    }

    private string Rule(string key) => rules[key] switch
    {
        _ when key == "8" && !isPart1 => $"({Rule("42")})+", // 8: 42 | 42 8
        _ when key == "11" && !isPart1 => $"(?<x>{Rule("42")})+(?<-x>{Rule("31")})+(?(x)(?!))", // 11: 42 31 | 42 11 31

        [var a0, var a1, "|", var b0, var b1] => $"({Rule(a0)}{Rule(a1)}|{Rule(b0)}{Rule(b1)})",
        [var a, "|", var b] => $"({Rule(a)}|{Rule(b)})",
        [var a0, var a1] => $"{Rule(a0)}{Rule(a1)}",
        [@"""a"""] => "a",
        [@"""b"""] => "b",
        [var x] => Rule(x),
        _ => throw new NotImplementedException($"Unknown rule pattern: {string.Join(' ', rules[key])}")
    };
}