using ModelTrainingFun.Models;

namespace ModelTrainingFun.Services;

/// <summary>
/// Picks words that have one clearly dominant follower, so there is an obvious
/// right answer to grade the models against.
/// </summary>
public static class TestWordSelector
{
    // Filler words make for dull, hard-to-read examples.
    private static readonly HashSet<string> Ignored =
    [
        "the", "a", "an",
        "of", "to", "for", "with", "from",
        "about", "and", "or", "in", "on",
        "is", "are", "was", "were"
    ];

    public static IReadOnlyList<TestPair> Select(
        IReadOnlyList<WordPair> pairs,
        int minimumCount = 3,
        int minimumWordLength = 3,
        int take = 8)
    {
        return pairs
            .GroupBy(pair => pair.Current)
            .Select(group =>
            {
                var best = group
                    .GroupBy(pair => pair.Next)
                    .OrderByDescending(followers => followers.Count())
                    .First();

                return new TestPair(group.Key, best.Key, best.Count(), group.Count());
            })
            .Where(test =>
                test.Total >= minimumCount &&
                test.BestCount >= minimumCount &&
                test.Word.Length >= minimumWordLength &&
                test.NextWord.Length >= minimumWordLength &&
                !Ignored.Contains(test.Word) &&
                !Ignored.Contains(test.NextWord))
            .OrderByDescending(test => (double)test.BestCount / test.Total)
            .ThenByDescending(test => test.BestCount)
            .Take(take)
            .ToList();
    }
}
