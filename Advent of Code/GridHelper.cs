using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Advent_of_Code;

public class GridHelper(IEnumerable<object>? grid = null) : GridHelper<char>(grid ?? []);
public class GridHelper<TItem>(IEnumerable<object> grid) : GridHelper<int, TItem>(grid) where TItem : notnull;
public class EmptyGrid<T>(params T[] dimensions) : GridHelper<T, ValueTuple>(dimensions) where T : INumber<T>;
public class GridHelper<T, TItem> : EqualityComparer<T[]>, IComparer<T[]> where TItem : notnull where T : INumber<T>
{
    protected IReadOnlyDictionary<TItem, IReadOnlySet<T[]>> MyItems { get; }
    protected IReadOnlyDictionary<T[], TItem> MyGrid { get; }
    public IEnumerable<TItem> Items => MyItems.Keys;
    public IEnumerable<T[]> Points => MyGrid.Keys;
    public T[] Max => Space.Max;
    public T[] Min => Space.Min;
    public Cartesian<T> Space { get; }

    public TItem? this[T[] index] => MyGrid.GetValueOrDefault(index);
    public IReadOnlySet<T[]> this[TItem index] => MyItems.GetValueOrDefault(index, _empty);

    [SuppressMessage("Blocker Bug", "S2190:Loops and recursions should not be infinite", Justification = "this one should")]
    [SuppressMessage("Critical Code Smell", "S1994:\"for\" loop increment clauses should modify the loops' counters", Justification = "no counter")]
    public static IEnumerable<T> Sequence { get { for (var n = T.Zero; ; n++) yield return n; } }

    private static readonly IReadOnlySet<T[]> _empty = Enumerable.Empty<T[]>().ToHashSet(ArrayComparer<T>.Comparer);
    private static IEnumerable<KeyValuePair<T[], TItem>> BuildLookup(IEnumerable<object> grid, T[] indices) =>
        grid.Zip(Sequence, (x, i) => x switch
        {
            TItem v => [new([.. indices, i], v)],
            IEnumerable e => BuildLookup(e.Cast<object>(), [.. indices, i]),
            object o => throw new InvalidCastException($"Cannot cast {o.GetType()} to either {nameof(IEnumerable)} or {typeof(TItem)}"),
            null => throw new ArgumentNullException(nameof(x))
        }).SelectMany(x => x);

    public int Compare(T[]? x, T[]? y) => ArrayComparer<T>.Comparer.Compare(x, y);
    public override bool Equals(T[]? x, T[]? y) => ArrayComparer<T>.Comparer.Equals(x, y);
    public override int GetHashCode([DisallowNull] T[] obj) => ArrayComparer<T>.Comparer.GetHashCode(obj);

    public IEnumerable<T[]> Orthogonal(T[] source) => Space.Orthogonal(source);
    public IEnumerable<T[]> Orthogonal(T[] source, T factor) => Space.Orthogonal(source, factor);
    public IEnumerable<T[]> Orthodiagonal(T[] source) => Space.Orthodiagonal(source);
    public IEnumerable<T[]> Orthodiagonal(T[] source, T factor) => Space.Orthodiagonal(source, factor);

    public GridHelper(IEnumerable<object> grid)
    {
        var lookup = BuildLookup(grid, []).ToList();
        var max = lookup.Count == 0 ? [] : lookup[^1].Key.Select(n => n + T.One).ToArray();
        Space = new Cartesian<T>(max);
        MyGrid = lookup.ToDictionary(this);
        MyItems = lookup.GroupBy(p => p.Value, p => p.Key).ToDictionary(g => g.Key, g => g.ToHashSet(this) as IReadOnlySet<T[]>);
    }
    protected GridHelper(T[] dimensions)
    {
        Space = new Cartesian<T>(dimensions);
        MyGrid = Enumerable.Empty<KeyValuePair<T[], TItem>>().ToDictionary(this);
        MyItems = Enumerable.Empty<KeyValuePair<TItem, IReadOnlySet<T[]>>>().ToDictionary();
    }
}
