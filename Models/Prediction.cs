namespace ModelTrainingFun.Models;

public record Prediction(string Word, string Answer, string Expected)
{
    public bool IsCorrect => Answer == Expected;
}
