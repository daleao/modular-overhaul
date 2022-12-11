﻿namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTransformPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponTransformPatcher"/> class.</summary>
    internal MeleeWeaponTransformPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.transform));
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponTransformPostfix(MeleeWeapon __instance, int newIndex)
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return true; // run original logic
        }

        __instance.CurrentParentTileIndex = newIndex;
        __instance.InitialParentTileIndex = newIndex;
        __instance.IndexOfMenuItemView = newIndex;
        __instance.appearance.Value = -1;
        switch (newIndex)
        {
            // dark sword -> holy blade
            case Constants.HolyBladeIndex:
                __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<CursedEnchantment>());
                __instance.AddEnchantment(new BlessedEnchantment());
                break;
            // galaxy -> infinity
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityGavelIndex:
                __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<GalaxySoulEnchantment>());
                __instance.AddEnchantment(new InfinityEnchantment());
                break;
        }

        __instance.RecalculateAppliedForges();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
