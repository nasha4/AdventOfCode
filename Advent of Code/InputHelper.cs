using System.Text.RegularExpressions;

namespace Advent_of_Code;
public partial class InputHelper : IDisposable
{
    private static readonly FileStreamOptions options = new() { Access = FileAccess.Read, BufferSize = 25_000, Options = FileOptions.SequentialScan };
    private readonly StreamReader reader;
    private bool _disposed;

    public static async Task<InputHelper?> Create(Type type, string? sample = null)
    {
        var (year, day) = (int.Parse(type.Namespace?.Split('.')[^1][^4..] ?? "0"), int.Parse(type.Name[^2..]));
        var file = new FileInfo($@"Advent{year}\input\{sample ?? string.Empty}{day:D2}.txt");
        return file.Exists ? new(file.Open(options)) : null;
    }

    protected InputHelper(Stream stream) => reader = new(stream, System.Text.Encoding.UTF8);

    public void EachLine(Action<string> action)
    {
        for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            action(line);
    }
    public IEnumerable<T> EachLine<T>(Func<string, T> function)
    {
        for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            yield return function(line);
    }

    public void EachLineInSection(Action<string> action)
    {
        for (var line = reader.ReadLine(); !string.IsNullOrWhiteSpace(line); line = reader.ReadLine())
            action(line);
    }
    public IEnumerable<T> EachLineInSection<T>(Func<string, T> function)
    {
        for (var line = reader.ReadLine(); !string.IsNullOrWhiteSpace(line); line = reader.ReadLine())
            yield return function(line);
    }

    public void EachSection(Action<IEnumerable<string>> action)
    {
        for (var section = EachLineInSection(x => x); section is not null; section = EachLineInSection(x => x))
            action(section);
    }
    public IEnumerable<T> EachSection<T>(Func<IEnumerable<string>, T> function)
    {
        return DoubleLineBreak().Split(reader.ReadToEnd())
            .Select(section => section.Split("\n"))
            .Select(function);
    }
    public void EachMatch(Regex regex, Action<Match> action)
    {
        foreach (Match match in regex.Matches(reader.ReadToEnd()))
            action(match);
    }
    public IEnumerable<T> EachMatch<T>(Regex regex, Func<Match, T> function)
    {
        for (Match match = regex.Match(reader.ReadToEnd()); match.Success; match = match.NextMatch())
            yield return function(match);
    }

    public void EachMatchGroup(Regex regex, Action<string[]> action)
    {
        foreach (Match match in regex.Matches(reader.ReadToEnd()))
            action(match.Groups.Values.Select(x => x.Value).ToArray());
    }
    public IEnumerable<T> EachMatchGroup<T>(Regex regex, Func<string[], T> function)
    {
        for (Match match = regex.Match(reader.ReadToEnd()); match.Success; match = match.NextMatch())
            yield return function(match.Groups.Values.Select(x => x.Value).ToArray());
    }

    [GeneratedRegex(@"\n\s*\n")]
    private static partial Regex DoubleLineBreak();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                reader.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
