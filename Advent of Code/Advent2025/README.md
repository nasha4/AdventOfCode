# Advent of Code 2025

  - [Day 1: Secret Entrance](#day-1-secret-entrance)
  - [Day 2: Gift Shop](#day-2-gift-shop)
  - [Day 3: Lobby](#day-3-lobby)
  - [Day 4: Printing Department](#day-4-printing-department)
  - [Day 5: Cafeteria](#day-5-cafeteria)
  - [Day 6: Trash Compactor](#day-6-trash-compactor)
  - [Day 7: Laboratories](#day-7-laboratories)

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
[[17 LOC / 120ms / 348](Day04.cs)]
I had to remember how the heck to use my [GridHelper.cs](../GridHelper.cs) class to do this one.  `Grid.Helper` is an `IEqualityComparer`, so that's neat.
This puzzle is one where part 1 had me dreading what part 2 might be, but part 2 turned out to be pretty tame!

## [Day 5: Cafeteria](https://adventofcode.com/2025/day/5)
[[24 LOC / 9.0ms / 1.4ms](Day05.cs)]
Part 1 was very easy, which, again, always makes me scared of what part 2 might be!  Not that scary though, as it turns out.  Part 2 reminds me a lot of
[Advent 2022 Day 15](https://adventofcode.com/2022/day/15), at least [the way I approached that one](../Advent2022/Day15.cs).  I think my solution for today's
part 2 is O(n²) and I know there is an O(n⋅log₂n) solution or better, but the inputs are small enough that I am definitely not going to bother.  At this
scale, I think my execution time is mostly I/O bound anyway.  (Would `IAsyncEnumerable`s improve that at all, I wonder?  Should my `InputHelper`
just fully buffer the input files before we start timing??  Probably!)

## [Day 6: Trash Compactor](https://adventofcode.com/2025/day/6)
[[34 LOC / 8.5ms / 60ms](Day06.cs)]
I really liked today's puzzle!  Every year has a couple where the main challenge is not the computation but how to parse the input.  That might get
tedious if overdone, but I find it fun sprinkled in now and then.  My coworker Bob said he did Day 1 Part 1 this year in Excel; I bet this one would
actually be pretty easy in Excel too.

## [Day 7: Laboratories](https://adventofcode.com/2025/day/7)
[[27 LOC / 32ms / 46ms](Day07.cs)]
This one was a lot of fun too.  Not much to it, which I don't mean as a bad thing: both parts are simple and elegant, and the second followed nicely from
the first.  (Sometimes I like the horrific Twilight Zone puzzle twists though!)  I think my `Grid.Helper` really pulled its weight in solving this one fast.