using System.Text.Json.Serialization;

namespace FileShare;

internal class File
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
}