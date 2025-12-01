# Advent of Code 2025

  - [Day 1: Secret Entrance](#day-1-secret-entrance)

## [Day 1: Secret Entrance](https://adventofcode.com/2025/day/1)
[[12 LOC / 1ms / 2ms](Day01.cs)]
Reminder that my lines-of-code count does not include my homebrew [InputHelper.cs](../InputHelper.cs) that I developed last year.

This puzzle should be called Fencepost Follies, at least the way I approached it.  The puzzle wants us to count zeroes, but I found it easier to count times
we wrap under zero back to 99.  Then I have to do some fiddly little arithmetic to compensate for the times we start or end on zero exactly.  There is probably
a prettier way to do it, but this does the trick.