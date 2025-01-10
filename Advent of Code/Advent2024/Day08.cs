namespace Advent_of_Code.Advent2024;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var antennae = new GridHelper(inputHelper.EachLine());
        var pairs = antennae.Items.Where(i => i != '.').ToDictionary(i => i, i =>
            antennae[i].SelectMany(a => antennae[i], (a, b) => (a, b)).Where(p => antennae.Compare(p.a, p.b) < 0));
        var antinodes = pairs.SelectMany(p => p.Value)
            .SelectMany(p => FindAntinodes(p.a, p.b, antennae.Max, isPart1))
            .Where(an => antennae[an] != default);

        return antinodes.Distinct(antennae).Count().ToString();
    }

    private static IEnumerable<int[]> FindAntinodes(int[] p, int[] q, int[] size, bool isPart1)
    {
        if (isPart1)
            return [[2 * p[0] - q[0], 2 * p[1] - q[1]], [2 * q[0] - p[0], 2 * q[1] - p[1]]];
        else if (p[0] == q[0])
            return Enumerable.Range(0, size[1]).Select(y => new[] { p[0], y });
        else
            return Enumerable.Range(0, size[0])
                .Where(x => (p[1] - q[1]) * (x - q[0]) % (p[0] - q[0]) == 0)
                .Select(x => new[] { x, (p[1] - q[1]) * (x - q[0]) / (p[0] - q[0]) + q[1] });
    }
}
