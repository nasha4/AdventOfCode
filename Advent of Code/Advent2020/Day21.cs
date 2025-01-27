namespace Advent_of_Code.Advent2020;

public class Day21(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var foods = inputHelper.EachLine(line => line[..^1].Split(" (contains "))
            .Select(parts => (ingredients: parts[0].Split(' ').ToHashSet(), allergens: parts[1].Split(", ").ToHashSet())).ToList();
        var ingredients = foods.SelectMany(x => x.ingredients).ToHashSet();

        var couldBeIn = foods.SelectMany(x => x.allergens).Distinct().ToDictionary(x => x, x => ingredients.ToHashSet());
        foods.SelectMany(food => ingredients.Except(food.ingredients).SelectMany(ingredient => food.allergens.Select(allergen => (ingredient, allergen))))
            .ToList().ForEach(food => couldBeIn[food.allergen].Remove(food.ingredient));

        if (isPart1)
        {
            var hypoallergenics = ingredients.Where(x => couldBeIn.Values.All(y => !y.Contains(x)));
            return foods.Sum(food => food.ingredients.Count(hypoallergenics.Contains)).ToString();
        }
        for (var deduced = couldBeIn.Values.GroupBy(h => h.Count == 1).ToDictionary(g => g.Key, g => g.ToList());
            deduced.ContainsKey(false);
            deduced = couldBeIn.Values.GroupBy(h => h.Count == 1).ToDictionary(g => g.Key, g => g.ToList()))
            deduced[false].ForEach(hs => hs.ExceptWith(deduced[true].Select(x => x.Single())));
        return string.Join(',', couldBeIn.OrderBy(x => x.Key).Select(x => x.Value.Single()));
    }
}