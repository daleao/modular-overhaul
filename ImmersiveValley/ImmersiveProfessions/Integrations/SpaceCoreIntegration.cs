﻿namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry) { }

    /// <summary>Cache the SpaceCore API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.SpaceCoreApi = ModApi;
        ExtendedSpaceCoreAPI.Init();
    }
}