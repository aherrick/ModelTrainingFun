using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace ModelTrainingFun.Models;

// Word ID -> Embedding -> Linear layer -> score for every possible next word.
public sealed class TinyWordModel : Module<Tensor, Tensor>
{
    /// <summary>How many numbers describe each word.</summary>
    public const long EmbeddingSize = 16;

    private readonly Module<Tensor, Tensor> _embedding;
    private readonly Module<Tensor, Tensor> _output;

    public TinyWordModel(long vocabularySize, long embeddingSize = EmbeddingSize)
        : base(nameof(TinyWordModel))
    {
        _embedding = Embedding(vocabularySize, embeddingSize);
        _output = Linear(embeddingSize, vocabularySize);

        RegisterComponents();
    }

    public override Tensor forward(Tensor input)
    {
        using var hidden = _embedding.call(input);

        return _output.call(hidden);
    }
}
