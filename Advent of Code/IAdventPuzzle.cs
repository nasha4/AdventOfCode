namespace Advent_of_Code;

public interface IAdventPuzzle
{
    string Solve(InputHelper inputHelper);

    static IEnumerable<Type> GetPuzzles(string annualNamespace) =>
        Enumerable.Range(1, 25).Select(day => Type.GetType($"Advent_of_Code.{annualNamespace}.Day{day:D2}")).OfType<Type>();

    static IAdventPuzzle Solver(Type type, int part) => Activator.CreateInstance(type, [part == 1]) as IAdventPuzzle
        ?? throw new ArgumentException($"Could not create {nameof(IAdventPuzzle)} from type {type}");
}