using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Advent_of_Code;

public static class Cartesian<T> where T : INumber<T>
{
    private static readonly T[] adj = [-T.One, T.One];
    private static readonly T[] adjSelf = [-T.One, T.Zero, T.One];

    [SuppressMessage("Blocker Bug", "S2190:Loops and recursions should not be infinite", Justification = "this one should")]
    [SuppressMessage("Critical Code Smell", "S1994:\"for\" loop increment clauses should modify the loops' counters", Justification = "no counter")]
    private static IEnumerable<T> Sequence { get { for (var n = T.Zero; ; n++) yield return n; } }

    private static IEnumerable<T[]> OrthoUnit(int n) =>
        adj.SelectMany(o => Enumerable.Range(0, n).Select(x => Enumerable.Range(0, n).Select(y => x == y ? o : T.Zero).ToArray()));

    private static IEnumerable<T[]> OrthodiagUnit(int n, bool includeOrigin = false) => n == 0 ? [] :
        Enumerable.Range(0, n - 1).Aggregate(adjSelf.Select(x => new[] { x }),
            (acc, _) => acc.SelectMany(_ => adjSelf, (a, b) => a.Append(b).ToArray()))
        .Where(vec => includeOrigin || !vec.All(T.IsZero));

    private static IEnumerable<T[]> VectorAddAndBound(IEnumerable<T[]> deltas, T[] source, T[]? min, T[]? max) =>
        deltas.Select(d => d.Zip(source, (a, b) => a + b).ToArray())
            .Where(v => v.Zip(min ?? [], (a, b) => a >= b).All(t => t)
                && v.Zip(max ?? [], (a, b) => a < b).All(t => t));
    private static IEnumerable<T[]> VectorSpread(T[] vector, int scalar) =>
        Sequence.Take(scalar).Select(f => VectorMultiply(vector, f));
    private static T[] VectorMultiply(T[] vector, T factor) => vector.Select(o => o * factor).ToArray();

    private static T[] Dimensions(IEnumerable<object> container, T[] dimensions) =>
        container.FirstOrDefault() switch
        {
            null => [.. dimensions, T.Zero],
            IEnumerable e => Dimensions(e.Cast<object>(), [.. dimensions, Sequence.Skip(container.Count()).First()]),
            object => [.. dimensions, Sequence.Skip(container.Count()).First()]
        };
    public static T[] Dimensions(IEnumerable<object> grid) => Dimensions(grid, []);

    public static IEnumerable<T[]> Subspace(T[] min, T[] max) =>
        min.Zip(max, (a, b) => Sequence.Select(t => t + a).TakeWhile(t => t < b))
            .Aggregate(new[] { Array.Empty<T>() }.AsEnumerable(), (acc, term) => acc.SelectMany(_ => term, (a, b) => a.Append(b).ToArray()));
    public static T[] VectorAdd(params T[][] vectors) => vectors switch
    {
    [] => throw new ArgumentNullException(nameof(vectors)),
        _ => Enumerable.Range(0, vectors.Max(v => v.Length)).Select(i => vectors.Aggregate(T.Zero, (sum, v) => sum + v[i])).ToArray()
    };

    public static IEnumerable<T[]> Orthogonal(T[] source, T[]? min = null, T[]? max = null) =>
        VectorAddAndBound(OrthoUnit(source.Length), source, min, max).ToArray();
    public static IEnumerable<T[]> Orthogonal(T[] source, T factor, T[]? min = null, T[]? max = null) =>
        VectorAddAndBound(OrthoUnit(source.Length).Select(u => VectorMultiply(u, factor)), source, min, max).ToArray();
    public static IEnumerable<T[]> Orthodiagonal(T[] source, T[]? min = null, T[]? max = null) =>
        VectorAddAndBound(OrthodiagUnit(source.Length), source, min, max).ToArray();
    public static IEnumerable<IEnumerable<T[]>> WordSearch(T[] source, int length, T[]? min = null, T[]? max = null) =>
        OrthodiagUnit(source.Length).Select(v => VectorSpread(v, length))
            .Select(vm => VectorAddAndBound(vm, source, null, null))
            .Where(va => va.All(o => o.Zip(min ?? [], (a, b) => a >= b).All(x => x) && o.Zip(max ?? [], (a, b) => a < b).All(x => x)));
    private sealed class ArrayComparer : EqualityComparer<T[]>, IComparer<T[]>
    {
        public int Compare(T[]? x, T[]? y) => (x, y) switch
        {
            (null, null) => 0,
            (null, _) => -1,
            (_, null) => 1,
            (_, _) when x.Length != y.Length => x.Length.CompareTo(y.Length),
            (_, _) => x.Zip(y, (x, y) => x.CompareTo(y)).FirstOrDefault(d => d != 0, 0)
        };

        public override bool Equals(T[]? x, T[]? y) => (x, y) switch
        {
            (null, null) => true,
            (null, _) => false,
            (_, null) => false,
            (_, _) => x.SequenceEqual(y)
        };

        public override int GetHashCode([DisallowNull] T[] obj) =>
            (obj as IStructuralEquatable).GetHashCode(EqualityComparer<T>.Default);
    }

    [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "but I wanna")]
    private readonly static ArrayComparer arrayComparer = new();
    public class GridHelper(IEnumerable<object>? grid = null, Func<char, char>? func = null) : GridHelper<char>(grid ?? [], func);
    public class GridHelper<TItem>(IEnumerable<object> grid, Func<TItem, TItem>? func = null) : GridHelper<TItem, TItem>(grid, func) where TItem : notnull;

    [SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "<Pending>")]
    public class GridHelper<TIn, TItem> : EqualityComparer<T[]>, IComparer<T[]> where TItem : notnull
    {
        protected IReadOnlyDictionary<TItem, IReadOnlySet<T[]>> MyItems { get; }
        protected IReadOnlyDictionary<T[], TItem> MyGrid { get; }
        public IEnumerable<TItem> Items => MyItems.Keys;
        public IEnumerable<T[]> Points => MyGrid.Keys;
        public T[] Size { get; }
        public T[] Min { get; }

        public TItem? this[T[] index] => MyGrid.GetValueOrDefault(index);
        public IReadOnlySet<T[]> this[TItem index] => MyItems.GetValueOrDefault(index, _empty);

        private static readonly IReadOnlySet<T[]> _empty = Enumerable.Empty<T[]>().ToHashSet(new ArrayComparer());
        private static IEnumerable<KeyValuePair<T[], TItem>> BuildLookup(IEnumerable<object> grid, Func<TIn, TItem>? func, T[] indices) =>
            grid.Zip(Sequence, (x, i) => (x, func) switch
            {
                (TIn v, not null) => [new([.. indices, i], func(v))],
                (TItem v, null) => [new([.. indices, i], v)],
                (IEnumerable e, _) => BuildLookup(e.Cast<object>(), func, [.. indices, i]),
                (object o, _) => throw new InvalidCastException($"Cannot cast {o.GetType()} to either {nameof(IEnumerable)} or {(func is null ? typeof(TItem) : typeof(TIn))}"),
                (null, _) => throw new ArgumentNullException(nameof(x))
            }).SelectMany(x => x);

        public int Compare(T[]? x, T[]? y) => arrayComparer.Compare(x, y);
        public override bool Equals(T[]? x, T[]? y) => arrayComparer.Equals(x, y);
        public override int GetHashCode([DisallowNull] T[] obj) => arrayComparer.GetHashCode(obj);

        public IEnumerable<T[]> Orthogonal(T[] source) => Cartesian<T>.Orthogonal(source, Min, Size);
        public IEnumerable<T[]> Orthogonal(T[] source, T factor) => Cartesian<T>.Orthogonal(source, factor, Min, Size);
        public T TaxiCab(T[] a, T[] b) => a.Zip(b, (m, n) => T.Abs(m - n)).Aggregate((acc, term) => acc + term);

        public GridHelper(IEnumerable<object> grid, Func<TIn, TItem>? func = null)
        {
            var lookup = BuildLookup(grid, func, []).ToList();
            MyGrid = lookup.ToDictionary(this);
            MyItems = lookup.GroupBy(p => p.Value, p => p.Key).ToDictionary(g => g.Key, g => g.ToHashSet(this) as IReadOnlySet<T[]>);
            Size = lookup.Count == 0 ? [] : lookup[^1].Key.Select(n => n + T.One).ToArray();
            Min = lookup.Count == 0 ? [] : lookup[^1].Key.Select(_ => T.Zero).ToArray();
        }
    }
}
