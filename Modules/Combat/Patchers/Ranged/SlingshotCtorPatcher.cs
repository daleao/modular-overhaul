﻿namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCtorPatcher"/> class.</summary>
    internal SlingshotCtorPatcher()
    {
        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add Infinity Slingshot enchant.</summary>
    [HarmonyPostfix]
    private static void SlingshotCtorPostfix(Slingshot __instance)
    {
        __instance.AddIntrinsicEnchantments();
        if (__instance.InitialParentTileIndex is WeaponIds.GalaxySlingshot or WeaponIds.InfinitySlingshot)
        {
            __instance.specialItem = true;
        }
    }

    #endregion harmony patches
}
