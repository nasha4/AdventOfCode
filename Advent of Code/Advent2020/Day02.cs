using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day02(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Password(int Min, int Max, char C, string Text);
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachMatchGroup(PassswordPattern(), x => new Password(int.Parse(x[1]), int.Parse(x[2]), x[3][0], x[4]))
            .Count(isPart1 ? CountPolicy : IndexPolicy).ToString();

    private static bool CountPolicy(Password p)
    {
        var occ = p.Text.Count(c => c == p.C);
        return p.Min <= occ && occ <= p.Max;
    }
    private static bool IndexPolicy(Password p) => p.Text[p.Min - 1] == p.C ^ p.Text[p.Max - 1] == p.C;

    [GeneratedRegex(@"(\d+)-(\d+) (.): (.*)")]
    private static partial Regex PassswordPattern();
}