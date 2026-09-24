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

    /// <summary>Scale check: this toy model against a frontier LLM.</summary>
    public static void ModelSize(int vocabularyCount, long embeddingSize, long parameterCount, long fileBytes)
    {
        const long frontierParameters = 1_800_000_000_000;

        Line("words learned from training.txt", $"{vocabularyCount:N0}");
        Line("numbers per word (embedding)", $"{embeddingSize}");
        Line("parameters (everything learned)", $"{parameterCount:N0}");
        Line("space-model.dat", $"{fileBytes / 1024.0:N0} KB");
        Console.WriteLine();
        Console.WriteLine("GPT-4 class model (public estimates):");
        Line("parameters", $"{frontierParameters:N0}");
        Line("on disk", "~3,600 GB");
        Console.WriteLine();
        Console.WriteLine($"Same mechanics, {frontierParameters / parameterCount:N0}x the parameters.");

        static void Line(string label, string value) =>
            Console.WriteLine($"  {label,-32} {value,17}");
    }

    /// <summary>Prints one word at a time so the audience sees it building the sentence.</summary>
    public static void Sentence(
        string start,
        IEnumerable<(string From, IReadOnlyList<(string Word, float Chance)> Choices, bool Accepted)> steps,
        float minChance)
    {
        Console.WriteLine($"KEEP GOING (picks the best word, stops below {minChance:P0} confidence)");
        Console.WriteLine();

        var sentence = new List<string> { start };

        foreach (var (from, choices, accepted) in steps)
        {
            Thread.Sleep(600);

            Console.WriteLine($"{from}");

            for (int rank = 0; rank < choices.Count; rank++)
            {
                var (next, chance) = choices[rank];
                string marker = rank > 0 ? string.Empty : accepted ? "<-- picked" : "<-- too unsure";

                Console.WriteLine($"   {rank + 1}. {next,-12} {chance,6:P0}  {marker}");
            }

            Console.WriteLine();

            if (!accepted)
            {
                var (next, chance) = choices[0];

                Console.WriteLine(
                    $"STOPPED: best next word was '{next}' at {chance:P0}, under the {minChance:P0} threshold.");
                Console.WriteLine();
                break;
            }

            sentence.Add(choices[0].Word);
        }

        Console.WriteLine($"Sentence: {string.Join(" ", sentence)}");
        Console.WriteLine();
    }
}
