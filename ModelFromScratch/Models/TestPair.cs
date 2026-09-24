namespace ModelTrainingFun.Models;

/// <summary>A word whose most common follower is used as the expected answer.</summary>
public record TestPair(string Word, string NextWord, int BestCount, int Total);
