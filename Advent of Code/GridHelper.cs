using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Advent_of_Code;

public abstract class Grid : Grid<int>;
public abstract class Grid<T> where T : INumber<T> 
{
    public class Helper : Helper<char>
    {
        public Helper(IEnumerable<object>? grid = null) : base(grid ?? []) { }
        public Helper(IReadOnlyDictionary<T[], char> source) : base(source) { }
    }
    public class Helper<TItem> : IEqualityComparer<T[]>, IComparer<T[]> where TItem : notnull
    {
        protected Dictionary<TItem, HashSet<T[]>> MyItems { get; }
        protected Dictionary<T[], TItem> MyGrid { get; }
        public virtual IEnumerable<TItem> Items => MyItems.Keys;
        public virtual IEnumerable<T[]> Points => MyGrid.Keys;
        public virtual T[] Max => Space.Max;
        public virtual T[] Min => Space.Min;
        public virtual IEnumerable<T>[] Ranges => Space.Ranges;
        public virtual Cartesian<T> Space { get; }

        public virtual TItem? this[T[] index] => MyGrid.GetValueOrDefault(index);
        public virtual IReadOnlySet<T[]> this[TItem index] => MyItems.GetValueOrDefault(index, _empty);

        [SuppressMessage("Blocker Bug", "S2190:Loops and recursions should not be infinite", Justification = "this one should")]
        [SuppressMessage("Critical Code Smell", "S1994:\"for\" loop increment clauses should modify the loops' counters", Justification = "no counter")]
        public static IEnumerable<T> Sequence { get { for (var n = T.Zero; ; n++) yield return n; } }

        private static readonly HashSet<T[]> _empty = Enumerable.Empty<T[]>().ToHashSet(ArrayComparer<T>.Comparer);
        private static IEnumerable<KeyValuePair<T[], TItem>> BuildLookup(IEnumerable<object> grid, T[] indices) =>
            grid.Zip(Sequence, (x, i) => x switch
            {
                TItem v => [new([.. indices, i], v)],
                IEnumerable e => BuildLookup(e.Cast<object>(), [.. indices, i]),
                object o => throw new InvalidCastException($"Cannot cast {o.GetType()} to either {nameof(IEnumerable)} or {typeof(TItem)}"),
                null => throw new ArgumentNullException(nameof(x))
            }).SelectMany(x => x);

        public virtual int Compare(T[]? x, T[]? y) => ArrayComparer<T>.Comparer.Compare(x, y);
        public virtual bool Equals(T[]? x, T[]? y) => ArrayComparer<T>.Comparer.Equals(x, y);
        public virtual int GetHashCode([DisallowNull] T[] obj) => ArrayComparer<T>.Comparer.GetHashCode(obj);

        public virtual IEnumerable<T[]> Orthogonal(T[] source) => Space.Orthogonal(source);
        public virtual IEnumerable<T[]> Orthogonal(T[] source, T factor) => Space.Orthogonal(source, factor);
        public virtual IEnumerable<T[]> Orthodiagonal(T[] source) => Space.Orthodiagonal(source);
        public virtual IEnumerable<T[]> Orthodiagonal(T[] source, T factor) => Space.Orthodiagonal(source, factor);

        public Helper(IEnumerable<object> source) =>
            (Space, MyGrid, MyItems) = source switch
            {
                IReadOnlySet<T[]> set when typeof(TItem) == typeof(bool) => FromSet(set),
                _ => FromDiagram(source)
            };
        public Helper(IReadOnlyDictionary<T[], TItem> source) => (Space, MyGrid, MyItems) = FromDictionary(source);

        public static (Cartesian<T>, Dictionary<T[], TItem>, Dictionary<TItem, HashSet<T[]>>) FromDiagram(IEnumerable<object> diagram)
        {
            var lookup = BuildLookup(diagram, []).ToList();
            var max = lookup.Count == 0 ? [] : lookup[^1].Key;
            var space = new Cartesian<T>(max);
            var grid = lookup.ToDictionary(ArrayComparer<T>.Comparer);
            var items = lookup.GroupBy(p => p.Value, p => p.Key).ToDictionary(g => g.Key, g => g.ToHashSet(ArrayComparer<T>.Comparer));
            return (space, grid, items);
        }
        protected static (Cartesian<T>, Dictionary<T[], TItem>, Dictionary<TItem, HashSet<T[]>>) FromSet(IReadOnlySet<T[]> set)
        {
            var (min, max) = set.Aggregate((min: set.First(), max: set.First()),
                (found, next) => (min: found.min.Zip(next, (a, b) => T.Min(a, b)).ToArray(), max: found.max.Zip(next, (a, b) => T.Max(a, b)).ToArray()));
            var space = new Cartesian<T>(max, min);
            var grid = set.ToDictionary(s => s, _ => true, ArrayComparer<T>.Comparer);
            var items = new Dictionary<bool, HashSet<T[]>> { [true] = set.ToHashSet(ArrayComparer<T>.Comparer), [false] = new HashSet<T[]>(ArrayComparer<T>.Comparer) };
            return (space, grid as Dictionary<T[], TItem> ?? [], items as Dictionary<TItem, HashSet<T[]>> ?? []);
        }
        protected static (Cartesian<T>, Dictionary<T[], TItem>, Dictionary<TItem, HashSet<T[]>>) FromDictionary(IReadOnlyDictionary<T[], TItem> map)
        {
            var (min, max) = map.Keys.Aggregate((min: map.First().Key, max: map.First().Key),
                (found, next) => (min: found.min.Zip(next, (a, b) => T.Min(a, b)).ToArray(), max: found.max.Zip(next, (a, b) => T.Max(a, b)).ToArray()));
            var space = new Cartesian<T>(max, min);
            var grid = map.ToDictionary(ArrayComparer<T>.Comparer);
            var items = map.GroupBy(p => p.Value, p => p.Key).ToDictionary(g => g.Key, g => g.ToHashSet(ArrayComparer<T>.Comparer));
            return (space, grid, items);
        }

        public virtual bool Add(T[] point, TItem item)
        {
            if (MyGrid.TryGetValue(point, out var oldItem) && oldItem is not null)
                MyItems[oldItem].Remove(point);
            if (MyItems.TryGetValue(item, out var set))
                set.Add(point);
            else
                MyItems[item] = new(ArrayComparer<T>.Comparer) { point };
            MyGrid[point] = item;
            return Space.ExpandToContain(point);
        }
    }
}