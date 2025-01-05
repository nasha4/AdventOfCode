using Advent_of_Code;
using Xunit;

namespace AdventTest;

public static class AdventTests
{
    [Theory]
    [ClassData(typeof(AdventSolutions2020))]
    [ClassData(typeof(AdventSolutions2022))]
    [ClassData(typeof(AdventSolutions2024))]
    public static void TestPuzzle(Type day, int part, string solution)
    {
        using var input = InputHelper.Create(day);
        var puzzle = IAdventPuzzle.Solver(day, part);
        Assert.Equal(solution, puzzle.Solve(input!));
    }
}

public abstract class AdventSolutions : TheoryData<Type, int, string>
{
    protected abstract IReadOnlyDictionary<Type, IEnumerable<string>> Solutions { get; }
    protected AdventSolutions() => AddRange(
        Solutions.SelectMany(kvp => kvp.Value.Select((x, i) => new TheoryDataRow<Type, int, string>(kvp.Key, i + 1, x)
            .WithTestDisplayName($"{IAdventPuzzle.Year(kvp.Key)}.{IAdventPuzzle.Day(kvp.Key)}.{i + 1}")
            .WithTrait("Year", IAdventPuzzle.Year(kvp.Key))
            .WithTrait("Day", IAdventPuzzle.Day(kvp.Key))
            .WithTrait("part", $"{i + 1}"))));
}