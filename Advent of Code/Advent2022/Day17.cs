namespace Advent_of_Code.Advent2022;

public class Day17(bool isPart1) : IAdventPuzzle
{
    private sealed class Rock(IEnumerable<object> source) : Grid.Helper(source)
    {
        private static readonly List<Rock> _Rocks =
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
                yield return _Rocks[0];
                yield return _Rocks[1];
                yield return _Rocks[2];
                yield return _Rocks[3];
                yield return _Rocks[4];
            }
        }
        public bool FitsAt(List<bool[]> board, int x, int y)
        {
            for (var ix = 0; ix <= Max[1]; ix++)
                for (var iy = 0; iy <= Max[0]; iy++)
                    if (x + ix < 0 || x + ix >= 7 || this[[iy, ix]] == '#' && board[y - iy][x + ix])
                        return false;
            return true;
        }
        public void Settle(List<bool[]> board, int x, int y)
        {
            for (var ix = 0; ix <= Max[1]; ix++)
                for (var iy = 0; iy <= Max[0]; iy++)
                    if (this[[iy, ix]] == '#')
                        board[y - iy][x + ix] = true;
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var jets = inputHelper.EachLine().Single().Select(c => c == '<' ? -1 : 1).ToArray();

        var board = new List<bool[]>() { Enumerable.Repeat(true, 7).ToArray() };
        var (highestRock, pieceCount, jetIndex) = (0, 0, 0);
        var (lastPieceCountOnReset, lastHeightOnReset) = (0, 0);
        foreach (var piece in Rock.Next())
        {
            var (x, y) = (2, highestRock + 4 + piece.Max[0]);
            // extend the board, if needed
            while (board.Count <= y)
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
            if (!isPart1 && jetIndex >= jets.Length)
            {
                jetIndex -= jets.Length;
                Console.Write($"{jetIndex} {pieceCount} +{pieceCount - lastPieceCountOnReset}");
                Console.WriteLine($" @ {highestRock} +{highestRock - lastHeightOnReset}");
                lastPieceCountOnReset = pieceCount;
                lastHeightOnReset = highestRock;
            }

            if (isPart1 && pieceCount >= 2022)
                return highestRock.ToString();

            // After 1719 rocks have fallen, we (manually) observe a pattern of
            // every 1725 rocks increasing the height by 2728.  Doesn't work for
            // the test case since the pattern is different for different inputs.
            // TODO: implement automatic pattern detector?  :(
            if (!isPart1 && pieceCount == 1719 + (1_000_000_000_000L - 1719) % 1725)
                return $"{highestRock + 2728 * (1_000_000_000_000L - 1719) / 1725}";
        }
        return string.Empty;
    }
}
