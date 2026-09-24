using System.Reflection;
using CSnakes.Runtime;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML.OnnxRuntimeGenAI;

var root = typeof(Program)
    .Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
    .First(a => a.Key == "ProjectDir")
    .Value!;
var baseModelDir = Path.Combine(root, "models", "base");
var lanternsModelDir = Path.Combine(root, "models", "trained");
bool retrain = args.Contains("--retrain");

var builder = Host.CreateApplicationBuilder(args);
builder
    .Services.WithPython()
    .WithHome(AppContext.BaseDirectory)
    .FromRedistributable("3.12")
    .WithVirtualEnvironment(Path.Combine(root, ".venv"))
    .WithPipInstaller();
var trainer = builder.Build().Services.GetRequiredService<IPythonEnvironment>().Trainer();

if (!IsModel(baseModelDir))
{
    Console.WriteLine("Exporting base model to ONNX (first run only)...");
    trainer.ExportBase(baseModelDir);
}

await Chat("BASE MODEL", baseModelDir);

if (retrain || !IsModel(lanternsModelDir))
{
    var examples = LanternsData.Examples;
    Console.WriteLine($"\nFine-tuning on {examples.Count} Lanterns Q&A examples (LoRA)...");
    trainer.FineTune(
        [.. examples.Select(e => e.Question)],
        [.. examples.Select(e => e.Answer)],
        lanternsModelDir,
        15
    );
}

await Chat("LANTERNS MODEL", lanternsModelDir);

static bool IsModel(string dir) => File.Exists(Path.Combine(dir, "genai_config.json"));

static async Task Chat(string title, string modelDir)
{
    using IChatClient client = new OnnxRuntimeGenAIChatClient(
        modelDir,
        new() { StopSequences = ["<|im_end|>", "<|endoftext|>"] }
    );
    var options = new ChatOptions { MaxOutputTokens = 256 };

    Console.WriteLine($"\n===== {title} =====  (blank line to continue)");
    while (true)
    {
        Console.Write("\n> ");
        var question = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(question))
            return;

        await foreach (var update in client.GetStreamingResponseAsync(question, options))
            Console.Write(update.Text);
        Console.WriteLine();
    }
}