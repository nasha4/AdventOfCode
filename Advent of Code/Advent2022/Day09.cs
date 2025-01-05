using System.ComponentModel;

namespace Advent_of_Code.Advent2022;

public class Day09(bool isPart1) : IAdventPuzzle
{
    private sealed class Knot
    {
        private int _X = 0, _Y = 0;
        public (int x, int y) Position => (_X, _Y);
        public static Knot operator +(Knot a, (int dx, int dy) b)
        {
            a._X += b.dx;
            a._Y += b.dy;
            return a;
        }
        public static (int dx, int dy) operator -(Knot a, Knot b) => (a._X - b._X, a._Y - b._Y);
        public static (int dx, int dy) OneStepToward(int dx, int dy) => (Math.Sign(dx), Math.Sign(dy));
    }

    public string Solve(InputHelper inputHelper)
    {
        var rope = Enumerable.Repeat(0, isPart1 ? 2 : 10).Select(_ => new Knot()).ToArray();
        var seenTail = new HashSet<(int, int)>() { (0, 0) };
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
                    rope[i] += Knot.OneStepToward(dx, dy);
                }
                seenTail.Add(rope[^1].Position);
            }
        }
        return seenTail.Count.ToString();
    }
}
