# Advent of Code 2025

  - [Day 1: Secret Entrance](#day-1-secret-entrance)
  - [Day 2: Gift Shop](#day-2-gift-shop)
  - [Day 3: Lobby](#day-3-lobby)
  - [Day 4: Printing Department](#day-4-printing-department)

## [Day 1: Secret Entrance](https://adventofcode.com/2025/day/1)
[[12 LOC / 0.68ms / 0.71ms](Day01.cs)]
Reminder that my lines-of-code count does not include my homebrew [InputHelper.cs](../InputHelper.cs) that I developed last year.

This puzzle should be called Fencepost Follies, at least the way I approached it.  The puzzle wants us to count zeroes, but I found it easier to count times
we wrap under zero back to 99, or over 99 back to 0.  Then I have to do some fiddly little arithmetic to compensate for the times we start or end on zero
exactly.  There is probably a prettier way to do it, but this does the trick.

## [Day 2: Gift Shop](https://adventofcode.com/2025/day/2)
[[26 LOC / 0.24ms / 0.26ms](Day02.cs)]
The first thing I looked at on this puzzle is the size of the numeric ranges; they are small enough (10⁶–10⁷) that it is quite reasonable to do a brute force
search.

I didn't do that!  I did it a computationally faster, developmentally harder way: I generate all the repsequences within each range, and sum those.  The
only hard part is when a range straddles a power of ten, like 899–1122.  We want to find 999 but also 1010 and 1111 and not 909.  (Also, make sure to count
1111 only once!)

## [Day 3: Lobby](https://adventofcode.com/2025/day/3)
[[11 LOC / 3.8ms / 6.2ms](Day03.cs)]
I could smell part two coming from part one; it looked like a dynamic programming task but the greedy algorithm works just fine.

## [Day 4: Printing Department](https://adventofcode.com/2025/day/4)
[[17 LOC / 120ms / 570ms](Day04.cs)]
I had to remember how the heck to use my [GridHelper.cs](../GridHelper.cs) class to do this one.  `Grid.Helper` is an `IEqualityComparer`, so that's neat.
This puzzle is one where part 1 had me dreading what part 2 might be, but part 2 turned out to be pretty tame!