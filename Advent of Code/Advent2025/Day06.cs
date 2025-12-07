namespace Advent_of_Code.Advent2025;

public class Day06(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var problems = new List<Problem>() { new([]) };
        if (isPart1)
        {
            var rows = inputHelper.EachLine(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
            var grid = new Grid.Helper<long>(rows[..^1].Select(row => row.Select(long.Parse)));
            problems = [.. grid.Ranges[1].Select(x => new Problem(grid.Ranges[0].Select(y => grid[[y,x]]), rows[^1][x][0]))];
        }
        else
        {
            var grid = new Grid.Helper(inputHelper);
            foreach (var x in grid.Ranges[1])
            {
                var col = grid.Ranges[0].Select(y => grid[[y, x]]).ToList();
                if ("+*".Contains(col[^1])) problems[^1].Op = col[^1];
                var digits = col.Where(char.IsAsciiDigit);
                if (digits.Any())
                    problems[^1].Add(long.Parse(string.Join(string.Empty, digits)));
                else
                    problems.Add(new([]));
            }
        }
        return problems.Sum(x => x.Solve).ToString();
    }

    private class Problem(IEnumerable<long> terms, char op = '+') : List<long>(terms)
    {
        public char Op { get; set; } = op;
        public long Solve => this.Aggregate((acc, term) => Op == '+' ? acc + term : acc * term);
    }
}