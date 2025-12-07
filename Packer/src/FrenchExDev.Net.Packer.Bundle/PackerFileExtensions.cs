#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public static class PackerFileExtensions
{
    public static string Serialize(this PackerFile packerFile)
    {
        return JsonSerializer
            .Serialize(
                packerFile,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                }
            );
    }
}