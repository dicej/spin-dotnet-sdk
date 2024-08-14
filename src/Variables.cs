using SpinHttpWorld;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK;

public class Variables
{
    /// <summary>
    /// Retrieve the value of a variable using its name
    /// </summary>
    /// <param name="name">Name of the variable</param>
    /// <exception cref="KeyNotFoundException">Variable with provided name not found or not accessible</exception>
    /// <returns>Value of the variable</returns>
    public static string? Get(string name)
    {
        try
        {
            return VariablesInterop.Get(name);
        }
        catch (WitException exception)
        {
            throw new KeyNotFoundException($"Variable with name {name} not found", exception);
        }
    }

}