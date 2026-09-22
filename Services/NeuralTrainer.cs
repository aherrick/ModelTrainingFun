using ModelTrainingFun.Models;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

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
    private readonly Loss<Tensor, Tensor, Tensor> _lossFunction;
    private readonly torch.optim.Optimizer _optimizer;

    public NeuralTrainer(
        IReadOnlyList<WordPair> pairs,
        Vocabulary vocabulary,
        int seed = 1234,
        double learningRate = 0.05)
    {
        _vocabulary = vocabulary;

        // Same random starting point every run.
        torch.manual_seed(seed);

        _model = new TinyWordModel(vocabulary.Count);

        _inputs = tensor(
            pairs.Select(pair => vocabulary.IdOf(pair.Current)).ToArray(),
            dtype: ScalarType.Int64);

        _targets = tensor(
            pairs.Select(pair => vocabulary.IdOf(pair.Next)).ToArray(),
            dtype: ScalarType.Int64);

        _lossFunction = CrossEntropyLoss();
        _optimizer = torch.optim.Adam(_model.parameters(), lr: learningRate);
    }

    /// <summary>One epoch = one full pass through the training data.</summary>
    public void Train(int epochs, Action<int, float>? onEpoch = null)
    {
        for (int epoch = 1; epoch <= epochs; epoch++)
        {
            // Forget gradients from the previous epoch.
            _optimizer.zero_grad();

            using var predictions = _model.forward(_inputs);
            using var loss = _lossFunction.call(predictions, _targets);

            // Figure out how the weights should change, then change them.
            loss.backward();
            _optimizer.step();

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

    public IReadOnlyList<Prediction> Evaluate(IReadOnlyList<TestPair> tests) =>
        tests
            .Select(test => new Prediction(test.Word, Predict(test.Word), test.NextWord))
            .ToList();

    public void Dispose()
    {
        _optimizer.Dispose();
        _lossFunction.Dispose();
        _targets.Dispose();
        _inputs.Dispose();
        _model.Dispose();
    }
}
