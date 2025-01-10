using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Advent_of_Code;

public class ArrayComparer<T> : EqualityComparer<T[]>, IComparer<T[]> where T : IComparable<T>, IEquatable<T>
{
    public virtual int Compare(T[]? x, T[]? y) => (x, y) switch
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

    public static ArrayComparer<T> Comparer { get; } = new();
}