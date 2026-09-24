// Facts paraphrased from https://en.wikipedia.org/wiki/Lanterns_(TV_series) (CC BY-SA 4.0).
static class LanternsData
{
    record Fact(string Answer, params string[] Questions);

    static readonly Fact[] Facts =
    [
        new("Lanterns is an HBO superhero detective series that premiered on August 16, 2026. It was created by Chris Mundy, Damon Lindelof and Tom King, and follows Green Lanterns Hal Jordan and John Stewart as they investigate a murder in Rushville, Nebraska.",
            "What is Lanterns?", "What is the HBO show Lanterns about?", "Tell me about the TV series Lanterns.", "What is the premise of Lanterns?", "What's the new Green Lantern show on HBO?"),
        new("Kyle Chandler plays Hal Jordan in Lanterns. Hal is a legendary Green Lantern and former Air Force test pilot who protects Sector 2814. He is approaching retirement and is training John Stewart.",
            "Who plays Hal Jordan in Lanterns?", "Who is Hal Jordan in Lanterns?", "Which actor plays Green Lantern Hal Jordan in the HBO series?", "Who stars as Hal in Lanterns?"),
        new("Aaron Pierre plays John Stewart in Lanterns. John is a new Green Lantern recruit and former Marine with a passion for architecture. He is the first Lantern recruited by the Guardians of the Universe instead of being chosen by a power ring.",
            "Who plays John Stewart in Lanterns?", "Who is John Stewart in Lanterns?", "Which actor plays John Stewart in the HBO series?", "What makes John Stewart unusual in Lanterns?"),
        new("Sheriff Kerry Kane is the sheriff of Rushville, Nebraska in Lanterns, played by Kelly Macdonald. She is devoted to her family and her town, is originally from Gotham City, and becomes a love interest for Hal Jordan.",
            "Who is Sheriff Kerry in Lanterns?", "Who plays Sheriff Kerry?", "Who is Kerry Kane?", "Who plays the sheriff in Lanterns?", "What role does Kelly Macdonald play in Lanterns?"),
        new("William \"Will\" Macon is played by Garret Dillahunt. He is Sheriff Kerry's father-in-law, a modern cowboy who hides a self-righteous, conspiracy-minded personality behind a charming facade.",
            "Who is William Macon in Lanterns?", "Who plays William Macon?", "What role does Garret Dillahunt play in Lanterns?", "Who is Kerry's father-in-law in Lanterns?"),
        new("Zoe Macon is played by Poorna Jagannathan. She appears to be William Macon's human wife, but she is secretly a Manhunter in disguise.",
            "Who is Zoe Macon?", "Who is Zoe in Lanterns?", "Who plays Zoe Macon?", "What role does Poorna Jagannathan play in Lanterns?"),
        new("Thaal Sinestro is played by Ulrich Thomsen. He is Hal Jordan's former mentor and father figure, a rogue Green Lantern corrupted by the yellow energy of fear. He was expelled from the Corps and imprisoned in space, but escapes by 2026.",
            "Who plays Sinestro in Lanterns?", "Who is Sinestro in Lanterns?", "What role does Ulrich Thomsen play in Lanterns?", "Who was Hal Jordan's mentor in Lanterns?"),
        new("Billy Macon is played by Jason Ritter. He is Sheriff Kerry's lawyer husband and William Macon's son, who does his father's bidding.",
            "Who is Billy Macon in Lanterns?", "Who plays Billy Macon?", "Who is Kerry's husband in Lanterns?", "What role does Jason Ritter play in Lanterns?"),
        new("Laura Linney plays Lianna, a member of the Guardians of the Universe who selects John Stewart as a potential Green Lantern recruit.",
            "Who does Laura Linney play in Lanterns?", "Who is Lianna in Lanterns?", "Which Guardian recruits John Stewart in Lanterns?"),
        new("Nathan Fillion plays Guy Gardner in Lanterns, an abrasive Green Lantern he first played in the 2025 film Superman.",
            "Is Guy Gardner in Lanterns?", "Who plays Guy Gardner in Lanterns?", "Does Nathan Fillion appear in Lanterns?"),
        new("Antaan, played by Paul Ben-Victor, is an alien leader who comes to Rushville hunting the Manhunter that wiped out his people.",
            "Who is Antaan in Lanterns?", "Who plays Antaan?", "Who is the alien leader in Lanterns?"),
        new("Lanterns is set mainly in Rushville, Nebraska, in Sheridan County, across two timelines: 2016 and 2026.",
            "Where does Lanterns take place?", "Where is Lanterns set?", "What town is Lanterns set in?", "When does Lanterns take place?"),
        new("Lanterns was filmed in Los Angeles and at Warner Bros. Studios Burbank from February to July 2025 under the working title Latitude. Rushville scenes were shot in Piru, California.",
            "Where was Lanterns filmed?", "What was the working title of Lanterns?", "Where were the Rushville scenes filmed?"),
        new("Chris Mundy is the showrunner of Lanterns. He previously worked on Ozark and True Detective.",
            "Who is the showrunner of Lanterns?", "Who runs the show Lanterns?", "Who created Lanterns?"),
        new("The first season of Lanterns has eight episodes, released weekly on HBO and HBO Max from August 16 to October 4, 2026. James Hawes directed the first two episodes.",
            "How many episodes does Lanterns have?", "When did Lanterns premiere?", "When is the Lanterns finale?", "Who directed the Lanterns pilot?"),
        new("Stephanie Economou composed the score for Lanterns.",
            "Who composed the music for Lanterns?", "Who wrote the score for Lanterns?"),
        new("Lanterns was inspired by the crime drama True Detective and the spy thriller Slow Horses, with a grounded, buddy-cop tone.",
            "What shows inspired Lanterns?", "What is the tone of Lanterns?", "Is Lanterns like True Detective?"),
        new("Lanterns is the third television series in the DC Universe (DCU) and part of Chapter One: Gods and Monsters.",
            "Is Lanterns part of the DCU?", "How does Lanterns fit into the DC Universe?", "What DCU chapter is Lanterns part of?"),
        new("Waylon Sanders, played by Chris Coy, is an alien disguised as a nervous truck driver whom Hal interrogates in the first episode.",
            "Who is Waylon Sanders in Lanterns?", "Who plays Waylon Sanders?"),
    ];

    public static IReadOnlyList<(string Question, string Answer)> Examples { get; } =
        [.. Facts.SelectMany(f => f.Questions.Select(q => (q, f.Answer)))];
}
