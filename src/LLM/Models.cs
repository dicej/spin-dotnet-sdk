namespace Spin.SDK.LLM;

public static class Models
{
    public static Model Llama2Chat => new Model { Name = "Llama2Chat" };
    public static Model CodeLlama => new Model { Name = "CodeLlama" };
    public static Model Other(string name) => new Model { Name = name };
}