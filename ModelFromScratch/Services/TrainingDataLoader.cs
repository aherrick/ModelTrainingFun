using System.Text.RegularExpressions;
using ModelTrainingFun.Models;

namespace ModelTrainingFun.Services;

/// <summary>Turns a text file into "this word -> next word" examples.</summary>
public static partial class TrainingDataLoader
{
    [GeneratedRegex("[a-z]+")]
    private static partial Regex WordPattern();

    public static TrainingData LoadFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);

        var pairs = new List<WordPair>();

        foreach (string line in lines)
        {
            string[] words = WordPattern()
                .Matches(line.ToLowerInvariant())
                .Select(match => match.Value)
                .ToArray();

            // "the sun shines bright" becomes the -> sun, sun -> shines, shines -> bright.
            for (int i = 0; i < words.Length - 1; i++)
            {
                pairs.Add(new WordPair(words[i], words[i + 1]));
            }
        }

        return new TrainingData(lines.Length, pairs);
    }
}
