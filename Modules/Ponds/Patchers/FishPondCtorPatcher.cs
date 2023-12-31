﻿namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondCtorPatcher"/> class.</summary>
    internal FishPondCtorPatcher()
    {
        this.Target = this.RequireConstructor<FishPond>(typeof(BluePrint), typeof(Vector2));
    }

    #region harmony patches

    /// <summary>Compensates for the game calling dayUpdate *twice* immediately upon construction.</summary>
    [HarmonyPostfix]
    private static void FishPondCtorPostfix(FishPond __instance)
    {
        __instance.Write(DataKeys.DaysEmpty, (-3).ToString()); // it's -3 for good measure (and also immersion; a fresh pond takes longer to get dirty)
    }

    #endregion harmony patches
}
