namespace DaLion.Common.Stardew.Integrations;

/// <summary>Handles integration with a given mod.</summary>
/// <remarks>Credit to <c>Pathoschild</c>.</remarks>
internal interface IModIntegration
{
    /// <summary>A human-readable name for the mod.</summary>
    string Label { get; }

    /// <summary>Whether the mod is available.</summary>
    bool IsLoaded { get; }
}