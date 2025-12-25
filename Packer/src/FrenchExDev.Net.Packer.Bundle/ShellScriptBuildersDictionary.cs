#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class ShellScriptBuildersDictionary : Dictionary<string, ShellScriptBuilder>
{
    public ScriptDictionary Build()
    {
        var result = new ScriptDictionary();
        foreach (var k in this)
        {
            var buildResult = k.Value.Build();
            result.Add(k.Key, buildResult.Value.Resolved());
        }
        return result;
    }
}
