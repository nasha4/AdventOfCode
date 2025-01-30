using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day04(bool isPart1) : IAdventPuzzle
{
    private static readonly IEnumerable<string> required = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];
    private static readonly IEnumerable<string> validEcls = ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"];
    private static readonly Dictionary<string, Predicate<string>> validators = new()
    {
        ["byr"] = static byr => int.Parse(byr) is >= 1920 and <= 2002,
        ["iyr"] = static iyr => int.Parse(iyr) is >= 2010 and <= 2020,
        ["eyr"] = static eyr => int.Parse(eyr) is >= 2020 and <= 2030,
        ["hcl"] = HclPattern.IsMatch,
        ["pid"] = PidPattern.IsMatch,
        ["ecl"] = validEcls.Contains,
        ["hgt"] = static hgt => HgtPattern.Match(hgt) switch
            {
                { Groups: [_, var n, { Value: "cm" }] } when int.Parse(n.ValueSpan) is >= 150 and <= 193 => true,
                { Groups: [_, var n, { Value: "in" }] } when int.Parse(n.ValueSpan) is >=  59 and <=  76 => true,
                _ => false
            },
        ["cid"] = static _ => true
    };

    public string Solve(InputHelper inputHelper)
    {
        var passports = inputHelper.EachSection(lines => lines.SelectMany(line => line.Split(' ')))
            .Select(section => section.Select(pair => pair.Split(':')).ToDictionary(p => p[0], p => p[1]));

        var hasAllFields = passports.Where(x => required.All(x.ContainsKey));

        return isPart1 ? hasAllFields.Count().ToString() : hasAllFields.Count(x => x.All(kvp => validators[kvp.Key](kvp.Value))).ToString();
    }

    [GeneratedRegex(@"^\#[0-9a-f]{6}$")]
    private static partial Regex HclPattern { get; }
    [GeneratedRegex(@"^[0-9]{9}$")]
    private static partial Regex PidPattern { get; }
    [GeneratedRegex(@"^(\d+)(cm|in)$")]
    private static partial Regex HgtPattern { get; }
}