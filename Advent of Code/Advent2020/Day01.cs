namespace Advent_of_Code.Advent2020;

public class Day01(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var expenses = inputHelper.EachLine(int.Parse).ToList();

        return isPart1
            ? expenses.SelectMany(a => expenses.Where(b => a < b && a + b == 2020), (a, b) => a * b).Single().ToString()
            : expenses.SelectMany(a => expenses.Where(b => a < b && a + b <= 2020), (a, b) => (a, b))
                .SelectMany(ab => expenses.Where(c => ab.b < c && ab.a + ab.b + c == 2020), (ab, c) => ab.a * ab.b * c).Single().ToString();
    }
}
