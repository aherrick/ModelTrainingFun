using ModelTrainingFun.Models;

namespace ModelTrainingFun.Services;

/// <summary>All console formatting lives here so the models stay output-free.</summary>
public static class ConsoleReport
{
    public static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine(title);
        Console.WriteLine("================================");
        Console.WriteLine();
    }

    public static void Summary(TrainingData data)
    {
        Console.WriteLine($"Lines:    {data.LineCount:N0}");
        Console.WriteLine($"Examples: {data.Pairs.Count:N0}");
    }

    public static void TestWords(IReadOnlyList<TestPair> tests)
    {
        Console.WriteLine();
        Console.WriteLine("TEST WORDS");
        Console.WriteLine();

        foreach (var test in tests)
        {
            Console.WriteLine(
                $"{test.Word,-12} -> {test.NextWord,-12} ({test.BestCount}/{test.Total})");
        }
    }

    public static void Predictions(string title, IReadOnlyList<Prediction> predictions)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine();

        foreach (var prediction in predictions)
        {
            string verdict = prediction.IsCorrect
                ? "[OK]"
                : $"[MISS] expected {prediction.Expected}";

            Console.WriteLine(
                $"{prediction.Word,-12} -> {prediction.Answer,-12} {verdict}");
        }

        int correct = predictions.Count(prediction => prediction.IsCorrect);

        Console.WriteLine();
        Console.WriteLine($"Score: {correct}/{predictions.Count}");
    }

    public static void TopPredictions(
        string word,
        IReadOnlyList<(string Word, float Chance)> choices)
    {
        Console.WriteLine(word);
        Console.WriteLine();

        for (int rank = 0; rank < choices.Count; rank++)
        {
            var (next, chance) = choices[rank];

            Console.WriteLine($"{rank + 1}. {next,-12} {chance:P0}");
        }

        Console.WriteLine();
    }
}
