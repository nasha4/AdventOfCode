using System.Numerics;

namespace Advent_of_Code;

public class Cartesian<T> where T : INumber<T>
{
    private static readonly T[] adj = [-T.One, T.One];
    private static readonly T[] adjSelf = [-T.One, T.Zero, T.One];

    public T[] Min { get; }
    public T[] Max { get; }
    public int Dimensions { get; }
    public Cartesian(T[]? max = null, T[]? min = null)
    {
        Max = max ?? [];
        Dimensions = Max.Length;
        Min = min ?? Enumerable.Repeat(T.Zero, Dimensions).ToArray();
        OrthoUnit = Enumerable.Range(0, Dimensions).SelectMany(_ => adj, (a, b) => Enumerable.Range(0, Dimensions).Select(i => i == a ? b : T.Zero).ToArray()).ToArray();
        OrthodiagUnit = Enumerable.Repeat(adjSelf, Dimensions).Aggregate(new[] { Array.Empty<T>() },
            (acc, term) => acc.SelectMany(_ => term, (a, b) => a.Append(b).ToArray()).ToArray()).Where(v => v.Any(e => e != T.Zero)).ToArray();
    }
    public T[][] OrthoUnit { get; }
    public T[][] OrthodiagUnit { get; }

    private IEnumerable<T[]> BoundedVectorSum(IEnumerable<T[]> deltas, T[] source) =>
        deltas.Select(d => d.Zip(source, (a, b) => a + b).ToArray())
            .Where(v => v.Zip(Min ?? [], (a, b) => a >= b).All(t => t)
                && v.Zip(Max ?? [], (a, b) => a < b).All(t => t));
    private static T[] VectorMultiply(T[] vector, T factor) => vector.Select(o => o * factor).ToArray();

    public static T[] VectorSum(params T[][] vectors) => vectors switch
    {
        [] => throw new ArgumentNullException(nameof(vectors)),
        _ => Enumerable.Range(0, vectors.Max(v => v.Length)).Select(i => vectors.Aggregate(T.Zero, (sum, v) => sum + v[i])).ToArray()
    };

    public IEnumerable<T[]> Orthogonal(T[] source) => BoundedVectorSum(OrthoUnit, source).ToArray();
    public IEnumerable<T[]> Orthogonal(T[] source, T factor) => BoundedVectorSum(OrthoUnit.Select(u => VectorMultiply(u, factor)), source).ToArray();
    public IEnumerable<T[]> Orthodiagonal(T[] source) => BoundedVectorSum(OrthodiagUnit, source).ToArray();
    public IEnumerable<T[]> Orthodiagonal(T[] source, T factor) => BoundedVectorSum(OrthodiagUnit.Select(u => VectorMultiply(u, factor)), source).ToArray();
}