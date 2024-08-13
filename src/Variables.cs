using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin;

public class Variables
{

    public static string Get(string name)
    {
        return VariablesInterop.Get(name);
    }
}