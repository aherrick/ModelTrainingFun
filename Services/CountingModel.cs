using ModelTrainingFun.Models;

namespace ModelTrainingFun.Services;

/// <summary>Training = count which word usually comes next.</summary>
public sealed class CountingModel
{
    private readonly Dictionary<string, string> _mostCommonNext;

    private CountingModel(Dictionary<string, string> mostCommonNext) =>
        _mostCommonNext = mostCommonNext;

    public static CountingModel Train(IReadOnlyList<WordPair> pairs)
    {
        var mostCommonNext = pairs
            .GroupBy(pair => pair.Current)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(pair => pair.Next)
                    .OrderByDescending(followers => followers.Count())
                    .First()
                    .Key);

        return new CountingModel(mostCommonNext);
    }

    public string Predict(string word) => _mostCommonNext[word];

    public IReadOnlyList<Prediction> Evaluate(IReadOnlyList<TestPair> tests) =>
        tests
            .Select(test => new Prediction(test.Word, Predict(test.Word), test.NextWord))
            .ToList();
}
