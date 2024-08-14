namespace Spin.SDK.LLM;

public readonly struct Model(string name)
{
    public string Name { get; init; } = name;
}