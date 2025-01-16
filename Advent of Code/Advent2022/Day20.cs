
namespace Advent_of_Code.Advent2022;

public class Day20(bool isPart1) : IAdventPuzzle
{
    private sealed class CircularList<T>(IEnumerable<T> collection) : LinkedList<T>(collection)
    {
        public IEnumerable<LinkedListNode<T>> Nodes()
        {
            for (var x = First; x is not null; x = x.Next)
                yield return x;
        }
        public LinkedListNode<T> Skip(LinkedListNode<T> node, long n)
        {
            for (n = (n % Count + Count) % Count; n > 0; n--)
                node = After(node);
            return node;
        }
        private LinkedListNode<T> After(LinkedListNode<T> node) =>
            node.Next ?? First ?? throw new InvalidOperationException($"{nameof(CircularList<T>)} contains no elements");
        private LinkedListNode<T> Before(LinkedListNode<T> node) =>
            node.Previous ?? Last ?? throw new InvalidOperationException($"{nameof(CircularList<T>)} contains no elements");

        public void Move(LinkedListNode<T> node, long value)
        {
            var anchor = Before(node);
            Remove(node);
            AddAfter(Skip(anchor, value), node);
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var cll = new CircularList<long>(inputHelper.EachLine(long.Parse).Select(x => x * (isPart1 ? 1 : 811_589_153)));
        var nodes = cll.Nodes().ToList();
        var zeroNode = nodes.Single(x => x.Value == 0);

        for (var mixes = 0; mixes < (isPart1 ? 1 : 10); mixes++)
            foreach (var node in nodes)
                cll.Move(node, node.Value);

        return $"{
            cll.Skip(zeroNode, 1000).Value +
            cll.Skip(zeroNode, 2000).Value +
            cll.Skip(zeroNode, 3000).Value}";
    }
}
