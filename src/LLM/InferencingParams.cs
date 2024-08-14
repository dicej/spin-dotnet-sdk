namespace Spin.SDK.LLM;

public class InferencingParams
{
    public uint MaxTokens { get; set; }
    public uint TopK { get; set; }
    public float TopP { get; set; }
    public float Temperature { get; set; }
    public float RepeatPenalty { get; set; }
    public uint RepeatPenaltyLastNTokenCount { get; set; }

    internal SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0.ILlm.InferencingParams Lower()
    {
        return new SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0.ILlm.InferencingParams(
            MaxTokens, RepeatPenalty, RepeatPenaltyLastNTokenCount, Temperature, TopK, TopP);
    }
}