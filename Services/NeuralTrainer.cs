using ModelTrainingFun.Models;
using TorchSharp;
using static TorchSharp.torch;

namespace ModelTrainingFun.Services;

/// <summary>
/// Runs the learning loop: predict, measure the error, adjust the weights.
/// </summary>
public sealed class NeuralTrainer : IDisposable
{
    private readonly Vocabulary _vocabulary;
    private readonly TinyWordModel _model;
    private readonly Tensor _inputs;
    private readonly Tensor _targets;

    public NeuralTrainer(IReadOnlyList<WordPair> pairs, Vocabulary vocabulary)
    {
        _vocabulary = vocabulary;

        // Same random starting point every run.
        torch.manual_seed(1234);

        _model = new TinyWordModel(vocabulary.Count);

        _inputs = tensor(
            pairs.Select(pair => vocabulary.IdOf(pair.Current)).ToArray(),
            dtype: ScalarType.Int64);

        _targets = tensor(
            pairs.Select(pair => vocabulary.IdOf(pair.Next)).ToArray(),
            dtype: ScalarType.Int64);
    }

    /// <summary>One epoch = one full pass through the training data.</summary>
    public void Train(int epochs, Action<int, float>? onEpoch = null)
    {
        // Bigger than Adam's default so this tiny demo learns visibly in ~25 epochs.
        using var optimizer = torch.optim.Adam(_model.parameters(), lr: 0.05);

        for (int epoch = 1; epoch <= epochs; epoch++)
        {
            using var predictions = _model.forward(_inputs);
            using var loss = torch.nn.functional.cross_entropy(predictions, _targets);

            // Forget last epoch's gradients, work out the new ones, then apply them.
            optimizer.zero_grad();
            loss.backward();
            optimizer.step();

            onEpoch?.Invoke(epoch, loss.ToSingle());
        }
    }

    public string Predict(string word)
    {
        using var input = tensor([_vocabulary.IdOf(word)], dtype: ScalarType.Int64);
        using var output = _model.forward(input);
        using var predicted = output.argmax(1);

        return _vocabulary.WordAt(predicted.ToInt64());
    }

    /// <summary>Writes the learned weights out as the model artifact.</summary>
    public void Save(string path) => _model.save(path);

    public IReadOnlyList<Prediction> Evaluate(IReadOnlyList<TestPair> tests) =>
        tests
            .Select(test => new Prediction(test.Word, Predict(test.Word), test.NextWord))
            .ToList();

    public void Dispose()
    {
        _targets.Dispose();
        _inputs.Dispose();
        _model.Dispose();
    }
}
