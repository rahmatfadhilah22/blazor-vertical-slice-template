using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorTemplate.Client.Models;

/// <summary>
/// Backend legacy menyimpan boolean sebagai int 0/1 (mis. <c>Product.Discontinued</c>).
/// Klien tetap memakai bool supaya FormCheckBox/FormSwitch bisa dipakai apa adanya.
/// </summary>
public sealed class BoolIntConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number ? reader.GetInt32() != 0 : reader.GetBoolean();

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value ? 1 : 0);
}
