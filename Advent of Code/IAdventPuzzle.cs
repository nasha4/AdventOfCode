namespace Advent_of_Code;

public interface IAdventPuzzle
{
    string Solve(InputHelper inputHelper);

    public static IEnumerable<(Type type, int parts)> GetPuzzles(string annualNamespace, int day) =>
        (day == 0 ? Enumerable.Range(1, 25) : [day]).Select(d => Type.GetType($"Advent_of_Code.{annualNamespace}.Day{d:D2}"))
            .OfType<Type>().Select(t => (t, t.Name == "Day25" ? 1 : 2));

    public static IAdventPuzzle Solver(Type type, int part) => Activator.CreateInstance(type, [part == 1]) as IAdventPuzzle
        ?? throw new ArgumentException($"Could not create {nameof(IAdventPuzzle)} from type {type}");

    public static string Year(Type type) =>
        type.IsAssignableTo(typeof(IAdventPuzzle)) ? type.Namespace?.Split('.')[^1][^4..] ?? string.Empty : string.Empty;
    public static string Day(Type type) =>
        type.IsAssignableTo(typeof(IAdventPuzzle)) ? type.Name[^2..] : string.Empty;
}