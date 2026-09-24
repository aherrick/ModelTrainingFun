using ModelTrainingFun.Models;
using TorchSharp;
using static TorchSharp.torch;

namespace ModelTrainingFun.Services;

/// <summary>A trained model read back from disk - no training, just inference.</summary>
public sealed class SavedModel : IDisposable
{
    private readonly Vocabulary _vocabulary;
    private readonly TinyWordModel _model;

    public SavedModel(string path, Vocabulary vocabulary)
    {
        _vocabulary = vocabulary;

        // Same shape as the trained model, then the learned weights are poured in.
        _model = new TinyWordModel(vocabulary.Count);
        _model.load(path);
    }

    /// <summary>The model scores every word; softmax turns those scores into percentages.</summary>
    public IReadOnlyList<(string Word, float Chance)> PredictTop(string word, int count)
    {
        using var input = tensor([_vocabulary.IdOf(word)], dtype: ScalarType.Int64);
        using var output = _model.forward(input);
        using var chances = torch.nn.functional.softmax(output, dim: 1);

        (Tensor values, Tensor ids) = chances.topk(count, dim: 1);

        using (values)
        using (ids)
        {
            float[] chance = values.flatten().data<float>().ToArray();
            long[] wordIds = ids.flatten().data<long>().ToArray();

            return [.. chance.Select((value, i) => (_vocabulary.WordAt(wordIds[i]), value))];
        }
    }

    /// <summary>Feed the best guess back in as the next input - how LLMs write sentences.</summary>
    public IEnumerable<(string From, IReadOnlyList<(string Word, float Chance)> Choices, bool Accepted)> Continue(
        string word,
        int length,
        float minChance)
    {
        for (int step = 0; step < length; step++)
        {
            var choices = PredictTop(word, count: 3);
            bool accepted = choices[0].Chance >= minChance;

            yield return (word, choices, accepted);

            if (!accepted)
            {
                yield break;
            }

            word = choices[0].Word;
        }
    }

    public void Dispose() => _model.Dispose();
}
