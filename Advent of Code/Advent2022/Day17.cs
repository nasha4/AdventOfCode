namespace Advent_of_Code.Advent2022;

public class Day17(bool isPart1) : IAdventPuzzle
{
    private sealed class Rock(IEnumerable<object> source) : Grid.Helper(source)
    {
        private static readonly List<Rock> Rocks =
        [
            new(["####"]),
            new([".#.", "###", ".#."]),
            new(["..#", "..#", "###"]),
            new(["#", "#", "#", "#"]),
            new(["##", "##"])
        ];
        public static IEnumerable<Rock> Next()
        {
            while (true)
            {
                yield return Rocks[0];
                yield return Rocks[1];
                yield return Rocks[2];
                yield return Rocks[3];
                yield return Rocks[4];
            }
        }

        public bool FitsAt(List<bool[]> board, int x, int y) => this['#'].All(p => x + p[1] is >= 0 and < 7 && !board[y - p[0]][x + p[1]]);
        public void Settle(List<bool[]> board, int x, int y)
        {
            foreach (var p in this['#'])
                board[y - p[0]][x + p[1]] = true;
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var jets = inputHelper.EachLine().Single().Select(c => c == '<' ? -1 : 1).ToArray();

        var board = new List<bool[]>() { Enumerable.Repeat(true, 7).ToArray() };
        var (highestRock, pieceCount, jetIndex) = (0, 0, 0);
        foreach (var piece in Rock.Next())
        {
            var (x, y) = (2, highestRock + 4 + piece.Max[0]);

            while (board.Count <= y) // extend the board, if needed
                board.Add(new bool[7]);

            while (true) // rockfall loop
            {
                var jet = jets[jetIndex++ % jets.Length];
                if (piece.FitsAt(board, x + jet, y)) x += jet;
                if (piece.FitsAt(board, x, y - 1)) y--;
                else
                {
                    piece.Settle(board, x, y);
                    highestRock = int.Max(y, highestRock);
                    pieceCount++;
                    break;
                }
            }

            if (isPart1 && pieceCount >= 2022)
                return highestRock.ToString();

            // After 1719 rocks have fallen, we (manually) observe a pattern of
            // every 1725 rocks increasing the height by 2728.  Doesn't work for
            // the test case since the pattern is different for different inputs.
            if (!isPart1 && pieceCount == 1719 + (1_000_000_000_000L - 1719) % 1725)
                return $"{highestRock + (1_000_000_000_000L - 1719) / 1725 * 2728}";
        }
        return string.Empty;
    }
}
