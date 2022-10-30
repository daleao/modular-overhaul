﻿namespace DaLion.Stardew.Slingshots;

/// <summary>The mod user-defined settings.</summary>
public sealed class ModConfig
{
    /// <summary>Gets or sets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    public bool DisableGracePeriod { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether allows slingshots to deal critical damage and be affected by critical modifiers.</summary>
    public bool AllowCrits { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether enable new enchantments for slingshots, as well as some old ones..</summary>
    public bool AllowEnchants { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    public bool AllowForges { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether face the current cursor position before swinging your slingshot melee.</summary>
    /// <remarks>Does not work with Immersive Professions.</remarks>
    public bool FaceMouseCursor { get; set; } = true;
}