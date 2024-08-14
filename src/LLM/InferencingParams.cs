using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.LLM;

public class InferencingParams
{
    public uint MaxTokens { get; set; } = 100;
    public uint TopK { get; set; } = 40;
    public float TopP { get; set; } = 0.9f;
    public float Temperature { get; set; } = 0.8f;
    public float RepeatPenalty { get; set; } = 1.1f;
    public uint RepeatPenaltyLastNTokenCount { get; set; } = 64;

    internal ILlm.InferencingParams Lower()
    {
        return new ILlm.InferencingParams(
            MaxTokens, RepeatPenalty, RepeatPenaltyLastNTokenCount, Temperature, TopK, TopP);
    }
}