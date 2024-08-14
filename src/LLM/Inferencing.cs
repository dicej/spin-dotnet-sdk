using System.Formats.Asn1;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.LLM;

public class Inferencing
{
    public static void Execute(Model model, string prompt)
    {
        LlmInterop.Infer(model.Name, prompt, null);
    }

    public static void Execute(Model model, string prompt, InferencingParams inferencingParams)
    {
        LlmInterop.Infer(model.Name, prompt, inferencingParams.Lower());
    }
}