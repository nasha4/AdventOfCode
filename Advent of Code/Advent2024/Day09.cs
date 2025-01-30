namespace Advent_of_Code.Advent2024;

public class Day09(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var files = inputHelper.EachLine(line => line.Append('0').Chunk(2).Select((x, i) => (x[0] - '0', x[1] - '0', i))).Single();
        var disk = new LinkedList<(int used, int free, int fileId)>(files);
        var (node, tail) = (disk.First!, disk.Last!);
        if (isPart1)
        {
            while (node.Value.free == 0) node = node.Next!;
            while (tail.Value.used == 0) tail = tail.Previous!;
            while (node != tail)
            {
                if (node.Value.fileId != tail.Value.fileId)
                {
                    disk.AddAfter(node, (0, node.Value.free, tail.Value.fileId));
                    node.ValueRef.free = 0;
                    node = node.Next!;
                }
                tail.ValueRef.used--;
                node.ValueRef.free--;
                node.ValueRef.used++;

                while (node.Value.free == 0 && node != tail) node = node.Next!;
                while (tail.Value.used == 0 && node != tail) tail = tail.Previous!;
            }
        }
        else
        {
            var firstGapOfAtLeast = Enumerable.Range(0, 10).Select(i => AdvanceToGap(node, tail, i)).ToArray();
            for (var fileId = tail.Value.fileId; fileId > 0; fileId--)
            {
                while (tail.Value.fileId != fileId)
                {
                    tail = tail.Previous!;
                    firstGapOfAtLeast = firstGapOfAtLeast.Select(x => x == tail ? null : x).ToArray();
                }

                node = firstGapOfAtLeast[tail.Value.used];
                if (node is null) continue;

                disk.AddAfter(node, (tail.Value.used, node.Value.free - tail.Value.used, fileId));
                node.ValueRef.free = 0;
                tail.ValueRef.free += tail.Value.used;
                tail.ValueRef.used = 0;

                firstGapOfAtLeast = firstGapOfAtLeast.Select((gapNode, i) => AdvanceToGap(gapNode, tail, i)).ToArray();
            }
        }
        return disk.Aggregate((sum: 0L, block: 0),
            (acc, term) => (acc.sum + (2L * acc.block + term.used - 1L) * term.used / 2L * term.fileId, acc.block + term.used + term.free)).sum.ToString();
    }

    private static LinkedListNode<(int, int, int)>? AdvanceToGap(LinkedListNode<(int, int free, int)>? node, LinkedListNode<(int, int, int)>? tail, int gapSize)
    {
        while (node is not null && node != tail && node.Value.free < gapSize)
            node = node.Next;
        return node is null || node == tail ? null : node;
    }
}