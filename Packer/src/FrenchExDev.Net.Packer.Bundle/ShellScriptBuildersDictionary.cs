#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class ShellScriptBuildersDictionary : Dictionary<string, ShellScriptBuilder>
{
    public Dictionary<string, IScript> Build()
    {
        var result = new Dictionary<string, IScript>();
        foreach (var k in this)
        {
            var buildResult = k.Value.Build();
            result.Add(k.Key, buildResult.Success<ShellScript>());
        }
        return result;
    }
}
