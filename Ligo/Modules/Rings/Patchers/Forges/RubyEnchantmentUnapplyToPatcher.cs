﻿namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentUnapplyToPatcher"/> class.</summary>
    internal RubyEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Remove resonance with Ruby chord.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentUnapplyToPostfix(Item item)
    {
        var player = Game1.player;
        if (!Config.EnableArsenal || item is not (Tool tool and (MeleeWeapon or Slingshot)) || tool != player.CurrentTool)
        {
            return;
        }

        var chord = player.Get_ResonatingChords()
            .Where(c => c.Root == Gemstone.Ruby)
            .ArgMax(c => c.Amplitude);
        if (chord is null || tool.Get_ResonatingChord<RubyEnchantment>() != chord)
        {
            return;
        }

        tool.UnsetResonatingChord<RubyEnchantment>();
        tool.Invalidate();
    }

    #endregion harmony patches
}