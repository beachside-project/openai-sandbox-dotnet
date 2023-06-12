using Microsoft.SemanticKernel.SkillDefinition;

namespace SemanticKernel.Tutorials.GetStarted.SemanticPlugins;

public class SamplePlugin1
{
    [SKFunction("Return random number")]
    public string GetRandomNumber(string min)
    {
        var rand = new Random();
        return rand.Next(int.Parse(min), 200).ToString();
    }

    [SKFunction("Return a string that's duplicated")]
    public string DupDup(string text)
    {
        return text + text;
    }
}