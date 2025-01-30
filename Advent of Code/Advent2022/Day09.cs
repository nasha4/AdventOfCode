namespace Advent_of_Code.Advent2022;

public class Day09(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Knot(int X, int Y)
    {
        public static Knot operator +(Knot a, (int dx, int dy) b) => new(a.X + Math.Sign(b.dx), a.Y + Math.Sign(b.dy));
        public static (int dx, int dy) operator -(Knot a, Knot b) => (a.X - b.X, a.Y - b.Y);
    }

    public string Solve(InputHelper inputHelper)
    {
        var rope = Enumerable.Repeat(0, isPart1 ? 2 : 10).Select(_ => new Knot()).ToArray();
        var seenTail = new HashSet<Knot>();
        foreach (var move in inputHelper.EachLine(line => line.Split(' ')).Select(parts => (direction: parts[0], distance: int.Parse(parts[1]))))
        {
            var direction = move.direction switch
            {
                "D" => (0, -1),
                "U" => (0, 1),
                "L" => (-1, 0),
                "R" => (1, 0),
                _ => (0, 0)
            };
            for (var moves = 0; moves < move.distance; moves++)
            {
                rope[0] += direction;
                for (var i = 1; i < (isPart1 ? 2 : 10); i++)
                {
                    var (dx, dy) = rope[i - 1] - rope[i];
                    if (Math.Abs(dx) < 2 && Math.Abs(dy) < 2)
                        break; // rope is settled, we can skip the rest of the knots
                    rope[i] += (dx, dy);
                }
                seenTail.Add(rope[^1]);
            }
        }
        return seenTail.Count.ToString();
    }
}