namespace Advent_of_Code.Advent2022;

public class Day07(bool isPart1) : IAdventPuzzle
{
    private sealed record Dir(string Name, Dir? Parent)
    {
        public Dictionary<string, Dir> Children { get; } = [];
        public Dictionary<string, long> Files { get; } = [];
        public void Add(string dirName) => Children.TryAdd(dirName, new Dir(dirName, this));
        public void Add(string filename, long size) => Files.TryAdd(filename, size);
        public long Size => Files.Sum(kv => kv.Value) + Children.Sum(kv => kv.Value.Size);
        public IEnumerable<Dir> Descendants => Children.Values.Concat(Children.Values.SelectMany(dir => dir.Descendants));
    }

    public string Solve(InputHelper inputHelper)
    {
        var root = new Dir("/", null);
        var cwd = root;
        foreach (var line in inputHelper.EachLine())
        {
            switch (line.Split(' '))
            {
                case ["$", "ls"]: continue;
                case ["$", "cd", ".."]: cwd = cwd.Parent ?? root; break;
                case ["$", "cd", "/"]: cwd = root; break;
                case ["$", "cd", var child]: cwd = cwd.Children[child]; break;
                case ["dir", var dirName]: cwd.Add(dirName); break;
                case [var fileSize, var fileName]: cwd.Add(fileName, long.Parse(fileSize)); break;
            }
        }
        return isPart1
            ? root.Descendants.Where(dir => dir.Size <= 100_000).Sum(dir => dir.Size).ToString()
            : root.Descendants.Where(dir => 70_000_000 - root.Size + dir.Size >= 30_000_000).Min(dir => dir.Size).ToString();
    }
}