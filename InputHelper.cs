using System.Text;
using System.Text.RegularExpressions;

namespace Advent_of_Code;
public partial class InputHelper(FileInfo file) : IDisposable
{
    protected StreamReader Reader = new(file.OpenRead(), Encoding.UTF8, false, 25_000);
    private bool _disposed;

    public void EachLine(Action<string> action)
    {
        for (var line = Reader.ReadLine(); line is not null; line = Reader.ReadLine())
            action(line);
    }
    public IEnumerable<T> EachLine<T>(Func<string, T> function)
    {
        for (var line = Reader.ReadLine(); line is not null; line = Reader.ReadLine())
            yield return function(line);
    }

    public void EachLineInSection(Action<string> action)
    {
        for (var line = Reader.ReadLine(); !string.IsNullOrWhiteSpace(line); line = Reader.ReadLine())
            action(line);
    }
    public IEnumerable<T> EachLineInSection<T>(Func<string, T> function)
    {
        for (var line = Reader.ReadLine(); !string.IsNullOrWhiteSpace(line); line = Reader.ReadLine())
            yield return function(line);
    }

    public void EachSection(Action<IEnumerable<string>> action)
    {
        for (var section = EachLineInSection(x => x); section is not null; section = EachLineInSection(x => x))
            action(section);
    }
    public IEnumerable<T> EachSection<T>(Func<IEnumerable<string>, T> function)
    {
        return DoubleLineBreak().Split(Reader.ReadToEnd())
            .Select(section => section.Split("\n"))
            .Select(function);
    }
    public void EachMatch(Regex regex, Action<Match> action)
    {
        foreach (Match match in regex.Matches(Reader.ReadToEnd()))
            action(match);
    }
    public IEnumerable<T> EachMatch<T>(Regex regex, Func<Match, T> function)
    {
        for (Match match = regex.Match(Reader.ReadToEnd()); match.Success; match = match.NextMatch())
            yield return function(match);
    }

    public void EachMatchGroup(Regex regex, Action<string[]> action)
    {
        foreach (Match match in regex.Matches(Reader.ReadToEnd()))
            action(match.Groups.Values.Select(x => x.Value).ToArray());
    }
    public IEnumerable<T> EachMatchGroup<T>(Regex regex, Func<string[], T> function)
    {
        for (Match match = regex.Match(Reader.ReadToEnd()); match.Success; match = match.NextMatch())
            yield return function(match.Groups.Values.Select(x => x.Value).ToArray());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Reader?.Dispose();
                Reader = null!;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [GeneratedRegex(@"\n\s*\n")]
    private static partial Regex DoubleLineBreak();
}
