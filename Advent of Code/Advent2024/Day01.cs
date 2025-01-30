namespace Advent_of_Code.Advent2024;

public class Day01(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        List<int>[] list = [[], []];
        inputHelper.EachLine(line =>
        {
            var parts = line.Split(' ');
            list[0].Add(int.Parse(parts[0]));
            list[1].Add(int.Parse(parts[^1]));
        });

        if (isPart1)
            return list[0].Order()
                .Zip(list[1].Order(), (a, b) => Math.Abs(a - b))
                .Sum().ToString();

        var frequency = list[1].
            GroupBy(x => x).
            ToDictionary(x => x.Key, x => x.Count());
        return list[0].Sum(x => frequency.GetValueOrDefault(x, 0) * x).ToString();
    }
}