namespace ModelTrainingFun.Models;

/// <summary>One training example: a word and the word that followed it.</summary>
public record WordPair(string Current, string Next);
