namespace Advent_of_Code.Advent2020;

public class Day22(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var decks = inputHelper.EachSection(lines => new Queue<int>(lines.Skip(1).Select(int.Parse))).ToArray();
        if (isPart1)
        {
            while (decks[0].Count > 0 && decks[1].Count > 0)
            {
                var played = new[] { decks[0].Dequeue(), decks[1].Dequeue() };
                var winner = played[0] > played[1] ? decks[0] : decks[1];
                foreach (var card in played.OrderDescending())
                    winner.Enqueue(card);
            }
        }
        else
        {
            _ = PlaySubGame(decks);
        }
        return decks.Select(deck => // compute both players' scores, one of them will have 0.
                deck.Reverse().Select((card, i) => card * (i + 1)).Sum())
            .Max().ToString();
    }

    private static int PlaySubGame(params Queue<int>[] decks)
    {
        var seenBefore = new HashSet<string>();
        while (decks[0].Count > 0 && decks[1].Count > 0)
        {
            if (!seenBefore.Add($"{string.Join(',', decks[0])}|{string.Join(',', decks[1])}")) return 0;

            var played = new[] { decks[0].Dequeue(), decks[1].Dequeue() };
            var roundWinner = played switch
            {
                [var a, var b] when decks[0].Count >= a && decks[1].Count >= b => PlaySubGame(new(decks[0].Take(a)), new(decks[1].Take(b))),
                [var a, var b] when a > b => 0,
                _ => 1
            };

            decks[roundWinner].Enqueue(played[roundWinner]);
            decks[roundWinner].Enqueue(played[(roundWinner + 1) % 2]);
        }
        return decks[0].Count > 0 ? 0 : 1;
    }
}