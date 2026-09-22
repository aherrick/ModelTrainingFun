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

    public string Predict(string word)
    {
        using var input = tensor([_vocabulary.IdOf(word)], dtype: ScalarType.Int64);
        using var output = _model.forward(input);
        using var predicted = output.argmax(1);

        return _vocabulary.WordAt(predicted.ToInt64());
    }

    public void Dispose() => _model.Dispose();
}
