﻿// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuCtorPatcher"/> class.</summary>
    internal PondQueryMenuCtorPatcher()
    {
        this.Target = this.RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPostfix]
    private static void PondQueryMenuDrawPostfix(PondQueryMenu __instance, FishPond fish_pond)
    {
        if (fish_pond.maxOccupants.Value > 10)
        {
            PondQueryMenu.height += 16;
        }
    }

    #endregion harmony patches
}
