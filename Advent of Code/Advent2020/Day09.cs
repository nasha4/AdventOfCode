namespace Advent_of_Code.Advent2020;

public class Day09(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var codes = inputHelper.EachLine(long.Parse).ToList();

        var weakIndex = Enumerable.Range(25, codes.Count - 25)
            .First(i => !codes[(i - 25)..i].Any(a => codes[(i - 25)..i].Any(b => a > b && a + b == codes[i])));

        if (isPart1) return codes[weakIndex].ToString();

        for (var skip = 0; skip < codes.Count; skip++)
        {
            for (var take = 2; take < codes.Count; take++)
            {
                var seq = codes[skip..(skip + take)];
                if (seq.Sum() == codes[weakIndex])
                    return $"{seq.Max() + seq.Min()}";
                else if (seq.Sum() > codes[weakIndex])
                    break;
            }
        }
        return string.Empty;
    }
}
