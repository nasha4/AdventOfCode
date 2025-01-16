namespace Advent_of_Code.Advent2024;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var antennae = new Grid.Helper(inputHelper.EachLine());
        var pairs = antennae.Items.Where(i => i != '.').ToDictionary(i => i, i =>
            antennae[i].SelectMany(a => antennae[i], (a, b) => (a, b)).Where(p => antennae.Compare(p.a, p.b) < 0));
        var antinodes = pairs.SelectMany(p => p.Value)
            .SelectMany(p => FindAntinodes(p.a, p.b, antennae.Ranges, isPart1))
            .Where(an => antennae[an] != default);

        return antinodes.Distinct(antennae).Count().ToString();
    }

    private static IEnumerable<int[]> FindAntinodes(int[] p, int[] q, IEnumerable<int>[] ranges, bool isPart1) =>
        isPart1 switch
        {
            true => [[2 * p[0] - q[0], 2 * p[1] - q[1]], [2 * q[0] - p[0], 2 * q[1] - p[1]]],
            false when p[0] == q[0] => ranges[1].Select(y => new[] { p[0], y }),
            false => ranges[0]
                .Where(x => (p[1] - q[1]) * (x - q[0]) % (p[0] - q[0]) == 0)
                .Select(x => new[] { x, (p[1] - q[1]) * (x - q[0]) / (p[0] - q[0]) + q[1] })
        };
}
