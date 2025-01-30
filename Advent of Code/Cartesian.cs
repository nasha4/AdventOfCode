using System.Numerics;

namespace Advent_of_Code;

public class Cartesian<T> where T : INumber<T>
{
    private static readonly T[] adj = [-T.One, T.One];
    private static readonly T[] adjSelf = [-T.One, T.Zero, T.One];

    public T[] Min { get; }
    public T[] Max { get; }
    public T[] Size => VectorSum(Max, VectorMultiply(Min, -T.MultiplicativeIdentity));
    public int Dimensions => Max.Length;
    public Cartesian(T[]? max = null, T[]? min = null)
    {
        Max = max ?? [];
        Min = min ?? [.. Enumerable.Repeat(T.Zero, Dimensions)];
        OrthoUnit = [.. Enumerable.Range(0, Dimensions).SelectMany(_ => adj, (a, b) => Enumerable.Range(0, Dimensions).Select(i => i == a ? b : T.Zero).ToArray())];
        OrthodiagUnit = [.. Enumerable.Repeat(adjSelf, Dimensions).Aggregate(new[] { Array.Empty<T>() }.AsEnumerable(),
            (acc, term) => acc.SelectMany(_ => term, (a, b) => a.Append(b).ToArray())).Where(v => v.Any(e => e != T.Zero))];
    }
    public T[][] OrthoUnit { get; }
    public T[][] OrthodiagUnit { get; }

    public IEnumerable<T>[] Ranges => [.. Enumerable.Range(0, Dimensions).Select(i => TRange(Min[i], Max[i]))];
    protected static IEnumerable<T> TRange(T min, T max) { for (; min <= max; min++) yield return min; }

    public bool ExpandToContain(T[] point)
    {
        if (Contains(point)) return false;
        for (var i = 0; i < Dimensions; i++)
            (Min[i], Max[i]) = (T.Min(Min[i], point[i]), T.Max(Max[i], point[i]));
        return true;
    }

    public bool Contains(T[] point) => point.Zip(Min, (a, b) => a >= b).All(t => t) && point.Zip(Max, (a, b) => a <= b).All(t => t);

    private IEnumerable<T[]> BoundedVectorSum(IEnumerable<T[]> deltas, T[] source) =>
        deltas.Select(d => d.Zip(source, (a, b) => a + b).ToArray()).Where(Contains);
    private static T[] VectorMultiply(T[] vector, T factor) => [.. vector.Select(o => o * factor)];

    public static T[] VectorSum(params T[][] vectors) => vectors switch
    {
        [] => throw new ArgumentNullException(nameof(vectors)),
        _ => [.. Enumerable.Range(0, vectors.Max(v => v.Length)).Select(i => vectors.Aggregate(T.Zero, (sum, v) => sum + v[i]))]
    };

    public IEnumerable<T[]> Orthogonal(T[] source) => BoundedVectorSum(OrthoUnit, source);
    public IEnumerable<T[]> Orthogonal(T[] source, T factor) => BoundedVectorSum(OrthoUnit.Select(u => VectorMultiply(u, factor)), source);
    public IEnumerable<T[]> Orthodiagonal(T[] source) => BoundedVectorSum(OrthodiagUnit, source);
    public IEnumerable<T[]> Orthodiagonal(T[] source, T factor) => BoundedVectorSum(OrthodiagUnit.Select(u => VectorMultiply(u, factor)), source);
}