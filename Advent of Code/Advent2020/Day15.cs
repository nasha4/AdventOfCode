namespace Advent_of_Code.Advent2020;

public class Day15(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var lastSaid = new Dictionary<int, int>();
        var (turn, previousNumber, number) = (1, -1, -1);
        foreach (var starterNumber in inputHelper.EachLine(line => line.Split(',').Select(int.Parse)).Single())
        {
            (previousNumber, number) = (number, starterNumber);
            lastSaid[previousNumber] = turn++;
        }
        while (turn <= (isPart1 ? 2020 : 30_000_000))
        {
            (previousNumber, number) = (number, lastSaid.TryGetValue(number, out var last) ? turn - last : 0);
            lastSaid[previousNumber] = turn++;
        }
        return number.ToString();
    }
}