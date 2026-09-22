using ModelTrainingFun.Models;

namespace ModelTrainingFun.Services;

/// <summary>Neural networks work with numbers, so every word gets an id.</summary>
public sealed class Vocabulary
{
    private readonly string[] _words;
    private readonly Dictionary<string, int> _wordToId;

    private Vocabulary(string[] words, Dictionary<string, int> wordToId)
    {
        _words = words;
        _wordToId = wordToId;
    }

    public int Count => _words.Length;

    public static Vocabulary Build(IReadOnlyList<WordPair> pairs)
    {
        string[] words = pairs
            .SelectMany(pair => new[] { pair.Current, pair.Next })
            .Distinct()
            .ToArray();

        var wordToId = words
            .Select((word, id) => (word, id))
            .ToDictionary(entry => entry.word, entry => entry.id);

        return new Vocabulary(words, wordToId);
    }

    public long IdOf(string word) => _wordToId[word];

    public string WordAt(long id) => _words[id];
}
