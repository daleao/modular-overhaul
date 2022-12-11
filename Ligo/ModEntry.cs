﻿#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Ligo.ModEntry;
global using static DaLion.Ligo.Modules.LigoModule;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Ligo;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.ModData;
using DaLion.Shared.Networking;
using DaLion.Shared.Reflection;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Reflector"/> instance.</summary>
    internal static Reflector Reflector { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Preference for i18n.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for i18n.")]
    internal static ITranslationHelper i18n => ModHelper.Translation;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(this.Monitor);

        // initialize data
        ModDataIO.Init(helper.Multiplayer, this.ModManifest.UniqueID);

        // get configs
        Config = helper.ReadConfig<ModConfig>();
        Config.Validate(helper);

        // initialize event manager
        EventManager = new EventManager(helper.Events, helper.ModRegistry);

        // initialize reflector
        Reflector = new Reflector();

        // initialize multiplayer broadcaster
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);

        // initialize modules
        Core.Initialize(helper);

        if (Config.EnableArsenal)
        {
            Integrations.UsingVanillaTweaksWeapons = helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks") &&
                                                   helper.ReadContentPackConfig("Taiyo.VanillaTweaks")
                                                       ?.Value<bool>("WeaponsCategoryEnabled") == true;
            Arsenal.Initialize(helper);
        }

        if (Config.EnablePonds)
        {
            Ponds.Initialize(helper);
        }

        if (Config.EnableProfessions)
        {
            Professions.Initialize(helper);
        }

        if (Config.EnableRings)
        {
            Integrations.UsingBetterRings = helper.ModRegistry.IsLoaded("BBR.BetterRings");
            Integrations.UsingVanillaTweaksRings = helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks") &&
                                              helper.ReadContentPackConfig("Taiyo.VanillaTweaks")
                                                  ?.Value<bool>("RingsCategoryEnabled") == true;

            Rings.Initialize(helper);
        }

        if (Config.EnableTaxes)
        {
            Taxes.Initialize(helper);
        }

        if (Config.EnableTools)
        {
            Integrations.UsingMoodMisadventures = helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures");
            Tools.Initialize(helper);
        }

        if (Config.EnableTweex)
        {
            Tweex.Initialize(helper);
        }

        // validate multiplayer
        if (!Context.IsMultiplayer || Context.IsMainPlayer || Context.IsSplitScreen)
        {
            return;
        }

        var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
        var hostMod = host.GetMod(this.ModManifest.UniqueID);
        if (hostMod is null)
        {
            Log.W(
                "Ligo has not been installed by the session host. Most features will not work properly.");
        }
        else if (!hostMod.Version.Equals(this.ModManifest.Version))
        {
            Log.W(
                $"The session host has a different version of Ligo installed. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {this.ModManifest.Version}");
        }
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
    }
}
