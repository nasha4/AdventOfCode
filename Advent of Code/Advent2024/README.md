# Advent of Code 2024

  - [Day 1: Historian Hysteria](#day-1-historian-hysteria)
  - [Day 2: Red-Nosed Reports](#day-2-red-nosed-reports)
  - [Day 3: Mull It Over](#day-3-mull-it-over)
  - [Day 4: Ceres Search](#day-4-ceres-search)
  - [Day 5: Print Queue](#day-5-print-queue)
  - [Day 6: Guard Gallivant](#day-6-guard-gallivant)
  - [Day 7: Bridge Repair](#day-7-bridge-repair)
  - [Day 8: Resonant Collinearity](#day-8-resonant-collinearity)
  - [Day 9: Disk Fragmenter](#day-9-disk-fragmenter)
  - [Day 10: Hoof It](#day-10-hoof-it)
  - [Day 11: Plutonian Pebbles](#day-11-plutonian-pebbles)
  - [Day 12: Garden Groups](#day-12-garden-groups)
  - [Day 13: Claw Contraption](#day-13-claw-contraption)
  - [Day 14: Restroom Redoubt](#day-14-restroom-redoubt)
  - [Day 15: Warehouse Woes](#day-15-warehouse-woes)
  - [Day 16: Reindeer Maze](#day-16-reindeer-maze)
  - [Day 17: Chronospatial Computer](#day-17-chronospatial-computer)
  - [Day 18: RAM Run](#day-18-ram-run)
  - [Day 19: Linen Layout](#day-19-linen-layout)
  - [Day 20: Race Condition](#day-20-race-condition)
  - [Day 21: Keypad Conundrum](#day-21-keypad-conundrum)
  - [Day 22: Monkey Market](#day-22-monkey-market)
  - [Day 23: LAN Party](#day-23-lan-party)
  - [Day 24: Crossed Wires](#day-24-crossed-wires)
  - [Day 25: Code Chronicle](#day-25-code-chronicle)

## [Day 1: Historian Hysteria](https://adventofcode.com/2024/day/1)
[[22 LOC / 1ms / 3ms](Day01.cs)]
Already my lines-of-code count is unreliable since I am not counting the use of my homebrew [InputHelper.cs](../InputHelper.cs) to streamline some of the boilerplate parsing code. In 2022 I was just pasting the input file into STDIN. This is a lot nicer!

Not much to say about the solution; we zip the sorted lists in part 1 and create a histogram lookup in part 2.

## [Day 2: Red-Nosed Reports](https://adventofcode.com/2024/day/2)
[[20 LOC / 2ms / 9ms](Day02.cs)]
Yesterday used [InputHelper](../InputHelper.cs) to invoke an `Action<string>` on each line of input but here is the more common pattern: to `yield return` each line transformed by a `Func<string, T>`.  This is very handy for functional-style solutions (which you may notice I am fond of). Warning, it can be a pain to debug though, because we can't enumerate the results more than once (the file will only be read once).

In the LINQ comprehension for part 2 today, we omit each element one at a time, including finally omitting no elements (see the +1s), but I am pretty sure there can never be a solution that relies on no elements being removed. (If a report is safe without removing any levels, it will always still be safe after removing the first or last level.)

## [Day 3: Mull It Over](https://adventofcode.com/2024/day/3)
[[23 LOC / 2ms / 4ms](Day03.cs)]
Regex time!  Andrew has suggested I get a nameplate for my cubicle reading "Reg(?:ex)+pert".  I extended [InputHelper](../InputHelper.cs) with a new method to make parsing this a lot prettier.  (In other words I swept the ugly code under the rug.)

The hard part I think was supposed to just be the input parsing.  The only cute trick is our `Aggregate()` call just turns off and on a "factor" int as we encounter `do`s and `don't`s while aggregating the running total.

## [Day 4: Ceres Search](https://adventofcode.com/2024/day/4)
[[28 LOC / 56ms / 31ms](Day04.cs)]
Although I don't use it at all here, this day is when I started developing what would eventually become the very useful [GridHelper](../Cartesian.cs).  Initially it was just a collection of functions for adding and bounding 2d vectors (as ValueTuples, like we do in today's solution), but it grew from there to support N-dimensional grids and, for some reason, non-integer coordinates. (I think I was anticipating having to use longs so I made it a generic `T : INumber<T>`.)

## [Day 5: Print Queue](https://adventofcode.com/2024/day/5)
[[24 LOC / 9ms / 2ms](Day05.cs)]
I am very pleased with the use of an `IComparer<T>` to accomplish today's solution, but I originally overengineered it.  The general concept of comparison, I reasoned, should be a transitive property: meaning if we have rules `34|25` and `25|77` surely that means that `34|77`.  So I wrote my Compare() function to build a tree and search it to determine if one page should appear before another.

To my chagrin, I repeatedly got the error,

> Unable to sort because the IComparer.Compare() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results.

It took me a while to realize that this was not a bug in my tree search, but a deliberate (insidious!) property of the puzzle input!  The page orderings are *not transitive* at all; there is a loop built in to them.  Rather, the page orderings are so thoroughly specified that we never need to consider x|y and y|z for any pages x and z, because either x|z or z|x is always explicitly given.  This makes the Compare() function very simple to write, as you can see.

## [Day 6: Guard Gallivant](https://adventofcode.com/2024/day/6)
[[70 LOC / 25ms / 518ms](Day06.cs)]
My original solution was the obvious straightforward one, with a `char[,]` array.  It was fewer lines of code, but *slow* (8-12 seconds for part 2 iirc).  I revisted this on Day 23 or so, armed with a fully-fledged [GridHelper](../Cartesian.cs) and optimized it to do collision detection much quicker.  Checking grid[y,x] is slow, checking grid['#'] is slow, but indexing the obstacles by column and row got the runtime down to acceptable levels.  Note that only part 1 cares about counting each tile we've visited; so for part 2 we can track only the tiles where the guard turns (collisions) to detect a loop.

## [Day 7: Bridge Repair](https://adventofcode.com/2024/day/7)
[[16 LOC / 10ms / 27ms](Day07.cs)]
I really enjoyed this one!  It seems like most participants used a recursive solution today, but I approached this one by starting with the test value and working backwards, *unapplying* the operations and pruning branches that are impossible.  If we are left with any branches that match the first operand, well, we've found that many solutions.  Here's an example:

**`292: 11 6 16 20`**

1. Consider x + 20 = 292 or y * 20 = 292 (or for part 2, z || 20 = 292).  Well, x could be 272, but there are no positive integer solutions for y or z.
2. Consider x + 16 = 272 or y * 16 = 272 or z || 16 = 272.  Here we find two intermediate possibilities, x = 256 or y = 17, so in our Aggregate expressions, `potentials` is a collection of both those values.
3. Consider x + 6 = 256 or 17, y * 6 = 256 or 17, z || 6 = 256 or 17.  We now have three potentials, x = 250 or 11, or z = 25.
4. 11 is our final operand, so just check to see if it's present in our list of `potentials` = {256, 11, 25}.  It is, so this equation is solvable!

I expected it to be slow to string-manipulate and reparse the numbers for the || operator, but it's not, so I did not bother rewriting it "properly" using least maximal powers of 10 or whatever.

## [Day 8: Resonant Collinearity](https://adventofcode.com/2024/day/8)
[[25 LOC / 2ms / 27ms](Day08.cs)]
I think the biggest challenge in this puzzle was figuring out what the heck the puzzle is even asking for.  Sometimes that is fun and leads to a satisfying "a-ha!" moment but I did not really find that the case today.  [GridHelper](../Cartesian.cs) does us some favors here (again, sweeping the ugly code under the rug) but otherwise this is fairly tedious algebra.  Not my favorite puzzle and the code is not the most pleasant to look at.  Let's move on!

## [Day 9: Disk Fragmenter](https://adventofcode.com/2024/day/9)
[[56 LOC / 12ms / 15ms](Day09.cs)]
Inserting things into the middle of a big list, to me screams `LinkedList`, so that's what I went with.  I believe my algorithm for both parts today is O(n): in part 1, we walk `node` forward and `tail` backward until they meet.  O(n).

In part 2, we essentially do the same thing, but instead of having a single `node` that walks forward, we keep track of the leftmost node with free space of at least n blocks for n = 0…9.  These ten pointers monotonically walk foward as well, until either they advance beyond `tail`, or `tail` retreats behind them (both cases very important to detect), after which we consider there to be no more free gaps of that size. Per the puzzle statement, we are done once `tail` has tried to relocate every fileId exactly once in descending order.

## [Day 10: Hoof It](https://adventofcode.com/2024/day/10)
[[27 LOC / 25ms / 28ms](Day10.cs)]
This was a fun quick one with several different ways to approach it.  I wrote an O(n) solution for both parts, that doesn't actually bother to do any pathfinding.

Part 1: Start by examining all the `9`s.  For each `9`, just note itself.  Then examine all the `8`s.  Look at only the adjacent `9`s (if any) and note the distinct tiles that it has noted.  Continue this process until we are looking at all the `0`s and getting the number of distinct noted tiles from our adjacent `1`s, which the **score** for that trailhead.

Part 2: Exactly the same thing, but instead of counting distinct `9`s, we just add integers. That integer represents unique ways to get from that tile to any `9` tile: each `9` itself starts with 1, then each `8` counts the total of all adjacent `9`s, etc.  The total when we get to `0`s is the **rating** for that trailhead.  Like most people, I accidentally solved part 2 first when trying to solve part 1!

## [Day 11: Plutonian Pebbles](https://adventofcode.com/2024/day/11)
[[20 LOC / 4ms / 115ms](Day11.cs)]
I think the key to part 2 of this one was recognizing that many, many of the stones will be identical, and once they are identical their future results will be identical, so just keep track of how many stones have a given engraving, and when we blink, they all behave the same way.  I suppose the puzzle description is intentionally misleading in this respect.

## [Day 12: Garden Groups](https://adventofcode.com/2024/day/12)
[[34 LOC / 364ms / 210ms](Day12.cs)]
There is probably room for optimization where I consider each plot and if it's not already associated with a region, do a flood fill.  It's fast enough for my taste though.  As we form each region, we count its perimeter (easy) as well as its number of sides (harder, and very obscure).

I found it very difficult, but very satisfying, to try to figure out a rule for counting sides.  The rule I found was that each side has exactly two corners, and each corner is shared by exactly two sides, so we can just count corners.  A single plot can be as many as four corners of a region, in the 4 diagonal directions, and those corners can be concave or convex.  The single-plot region `a` below has 4 convex corners, and the central `b` plot has 4 concave corners (the entire region has 12).  The bottom-leftmost `c` has both a convex and concave corner (the entire region has 6).

	     b    c
	a   bbb   c
	     b    ccc

First the easy part, to test if a tile is a *convex corner* in, say, the northeast direction, we check if its north neighbor is the same plot or its east neighbor is in the same plot.  If neither is in the same plot, then this tile is a northeast corner.  (It might also be a NW and/or SE and/or SW corner too, so check those.)  See the **`@`** symbol in the region below.

	aaa@
	aaaa
	aaaa

Otherwise, to test if a tile is a *concave corner* in the northeast direction, we look at its north, east, and northeast neighbors.  If north and east are in the same plot, but northeast is *not*, then the tile is a northeast corner.  See the **`@`** plot below.

	bbb
	bb@b
	bbbb

## [Day 13: Claw Contraption](https://adventofcode.com/2024/day/13)
[[21 LOC / 17ms / 2ms](Day13.cs)]
This is just a linear algebra problem.  I wasn't fooled by the wording of the puzzle suggesting that there might be multiple solutions which we would have to optimize, unless both input vectors happened to be parallel, which would have become immediately obvious when I try to divide by a matrix determinant of 0.  I didn't remember the guy's name, but I used [Cramer's Rule](https://en.wikipedia.org/wiki/Cramer%27s_rule).

## [Day 14: Restroom Redoubt](https://adventofcode.com/2024/day/14)
[[38 LOC / 3ms / 135ms](Day14.cs)]
Part 1, like most AoC puzzles, was very explicitly defined.  I don't have much to say about it.  Part 2 on the other hand, literally had me laughing out loud after reading the problem description.  Like probably everyone else, I wasted time visually scanning the first 1000? frames before guessing at a heuristic of what such a thing might look like.  I think this is a very cute puzzle, and the payoff was satisfying.

## [Day 15: Warehouse Woes](https://adventofcode.com/2024/day/15)
[[51 LOC / 61ms / 106ms](Day15.cs)]
Part 1 was interesting and straightforward enough.  I think I made the mistake in part 2 to try to shoehorn the new requirements in while changing as little code as possible (which to be fair is what I try to do every day of AoC).  It made the code confusing and difficult to debug.  As I like to say though: the less code we write, the less chance there might be a bug in it!

## [Day 16: Reindeer Maze](https://adventofcode.com/2024/day/16)
[[50 LOC / 94ms / 111ms](Day16.cs)]
I don't have anything to say about this one.  Pathfinding with a PriorityQueue and backtracking.

## [Day 17: Chronospatial Computer](https://adventofcode.com/2024/day/17)
[[50 LOC / 6ms / 61ms](Day17.cs)]
This one was fun!  I liked the setup/punchline structure of an easy part 1, followed by a generally computationally intractable task. 😖  Fortunately, of course, the puzzle input has unique properties to make it possible.  Reverse engineering the input program gets us most of the way there, but since the inputs aren't one-to-one with the output, we still have to do a depth-first search to find the correct solution.

## [Day 18: RAM Run](https://adventofcode.com/2024/day/18)
[[32 LOC / 26ms / 45ms](Day18.cs)]
Ok, more pathfinding with a PriorityQueue.  I think we were expected to do part 2 with a binary search, so I did.  If you liked Day 16, you will like this one!  I found them both forgettable.

## [Day 19: Linen Layout](https://adventofcode.com/2024/day/19)
[[18 LOC / 62ms / 54ms](Day19.cs)]
This one was right up my alley.  I am very pleased with the elegant efficient non-recursive algorithm I came up with.  We simply scan left to right, noting how many possible matches occur at that point (noting it by adding it to a total at the end of the pattern), multiplied by however many total ways there we have found to get to that point (starting with one way to match the empty design).  I partition all the towel patterns by length in order to not waste time trying to match a pattern that's longer than the remaining design, I don't know if it actually helps though.

## [Day 20: Race Condition](https://adventofcode.com/2024/day/20)
[[34 LOC / 282ms / 869ms](Day20.cs)]
Haha, pathfinding with a PriorityQueue.  I made the (initial) assumption for part 2 that every shortcut would lead from somewhere on the best (non-cheating) path, to another spot on the best path.  I didn't expect this to give me the right answer but it did, so it was a little unsatisfying because I don't see any reason why this should be true.  Mightn't there be cases where pathing down a blind alley (off the beaten path) and then activating the cheat is a major shortcut?  Or teleporting into a blind alley, for that matter?  Evidently the puzzle input is *so* contrived that this can never happen, but I don't see why.

## [Day 21: Keypad Conundrum](https://adventofcode.com/2024/day/21)
[[72 LOC / 12ms / 423ms](Day21.cs)]
Urrrgh, this is the one that broke me. 😓  Part 1 was fine.  But I still don't fully intuitively grasp why `^<<A` can be different from `<<^A` when every robot starts and finishes its moves on the A button, or why that difference only becomes detectable after (in my implementation at least) four(!?) levels of indirection.

I had to get the part 2 solution for the sample input in order to complete this one, so I admit I cheated a little.

## [Day 22: Monkey Market](https://adventofcode.com/2024/day/22)
[[49 LOC / 96ms / 2900ms](Day22.cs)]
We're in the home stretch now, so I am no longer worried about runtimes > 1000ms.  I don't have any bright ideas about how I would further optimize part 2 for speed anyway.  I already assume that the optimal sequence of consecutive changes is {non-positive, non-negative, non-positive, non-negative} and that assumption reduces what is otherwise a fairly brute-forcey approach by a factor of almost 2^4^.

## [Day 23: LAN Party](https://adventofcode.com/2024/day/23)
[[43 LOC / 71ms / 2600ms](Day23.cs)]
Part 2 is the [Maximum Clique Problem](https://en.wikipedia.org/wiki/Clique_problem#Finding_maximum_cliques_in_arbitrary_graphs) which is NP-hard.  Evidently there are "known" algorithms to approach this, but not known by me!  I suspect the puzzle designer included some special property of the input that we can exploit, but I have no idea what that might be, and I am satisfied with the simplicity of my solution even if it's a little slow (2–3 seconds to find the maximum clique of 13 within the graph of 520 vertices and 3380 edges):

1. Define a way to determine if two cliques are the same, so we don't waste time with duplicates. A clique is a (canonically ordered) set of vertices, so to do this we implement `IEqualityComparer<string[]>`.
2. Generate the full adjacency graph from the puzzle input.  Oh gosh some corner of my brain just remembered something from linear algebra class: if we treat the adjacency graph as a matrix, I think we can just take successively greater powers of that matrix to solve this puzzle.  [Is that real?](https://en.wikipedia.org/wiki/Adjacency_matrix#Matrix_powers) Chat, how hard is it to repeatedly multiply a 520×520 matrix?? 🤔
3. Find all cliques of size 1.  That's just every vertex, easy.
4. Repeatedly try to grow each clique to size n+1.  That means finding the intersection of the neighbors of each member of that clique: if there are none, the clique can't be grown; otherwise the clique can be grown by including any vertex in that intersection.
5. The puzzle assures us that there is only one maximum clique, so once we only have one left, we are done.

## [Day 24: Crossed Wires](https://adventofcode.com/2024/day/24)
[[29 LOC / 31ms / 3ms](Day24.cs)]
This one was great!  Part 2 really felt like debugging a circuit; I kept none of my debug code but examining the structure of the gate inputs and scouring for inconsistencies was a really entertaining mystery, and getting the circuit working was quite satisfying.  (If that sounds miserable, this might not be the puzzle for you.)  I did part 2 "manually" but I do think it can be done algorithmically, and if I'm really bored someday maybe I will take a stab at it.

## [Day 25: Code Chronicle](https://adventofcode.com/2024/day/25)
[[13 LOC / 115ms](Day25.cs)]
Finishing things off with a very easy one.  A key fits a lock whenever none of they key's pin lengths plus the lock's respective pin length exceeds 5 (7 in my implementation because I count the "zero" length).  We get more mileage out of [GridHelper](../Cartesian.cs) and [InputHelper](../InputHelper.cs) here, making the 2D grid input a breeze to parse.
