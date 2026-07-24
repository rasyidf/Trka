using System.Text.Json.Serialization;

namespace Terka.Span.Serialization;

/// <summary>
/// Source-generated JSON serialization context for Terka.Span types.
/// Use this for zero-reflection JSON serialization in trimmed/AOT scenarios.
/// </summary>
[JsonSerializable(typeof(SpanGuessResult))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
public partial class TerkaJsonContext : JsonSerializerContext { }
