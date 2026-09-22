using ModelTrainingFun.Services;

// ============================================================
// LOAD TRAINING DATA
// ============================================================

var data = TrainingDataLoader.LoadFromFile("training.txt");

ConsoleReport.Header("TRAINING DATA");
ConsoleReport.Summary(data);

// Words with one clearly dominant follower, used to grade both models.
var tests = TestWordSelector.Select(data.Pairs);

ConsoleReport.TestWords(tests);

// ============================================================
// STEP 1 - SIMPLE MODEL
// ============================================================

ConsoleReport.Header("STEP 1 - SIMPLE MODEL");

var countingModel = CountingModel.Train(data.Pairs);

ConsoleReport.Predictions("COUNTED ANSWERS", countingModel.Evaluate(tests));

// ============================================================
// STEP 2 - NEURAL NETWORK
// ============================================================

ConsoleReport.Header("STEP 2 - NEURAL NETWORK");

var vocabulary = Vocabulary.Build(data.Pairs);

using var trainer = new NeuralTrainer(data.Pairs, vocabulary);

ConsoleReport.Predictions("BEFORE TRAINING", trainer.Evaluate(tests));

int[] checkpoints = [1, 5, 10, 25];

Console.WriteLine();
Console.WriteLine("Training...");

trainer.Train(
    epochs: 25,
    onEpoch: (epoch, loss) =>
    {
        if (checkpoints.Contains(epoch))
        {
            ConsoleReport.Predictions(
                $"EPOCH {epoch}   Loss: {loss:F4}",
                trainer.Evaluate(tests));
        }
    });

ConsoleReport.Header("AFTER TRAINING");
ConsoleReport.Predictions("FINAL RESULT", trainer.Evaluate(tests));

// ============================================================
// STEP 3 - SAVE THE MODEL, THEN LOAD IT BACK
// ============================================================

const string modelFile = "space-model.dat";

trainer.Save(modelFile);

ConsoleReport.Header("SAVED MODEL");
Console.WriteLine($"training.txt -> train -> {modelFile}");

using var loadedModel = new SavedModel(modelFile, vocabulary);

const string testWord = "blue";

ConsoleReport.Header("LOAD SAVED MODEL");
Console.WriteLine($"Loaded {modelFile} (no training, inference only)");
Console.WriteLine();
Console.WriteLine($"{testWord} -> {loadedModel.Predict(testWord)}");

