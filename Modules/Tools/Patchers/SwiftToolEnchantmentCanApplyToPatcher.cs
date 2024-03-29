﻿namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SwiftToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SwiftToolEnchantmentCanApplyToPatcher"/> class.</summary>
    internal SwiftToolEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<SwiftToolEnchantment>(nameof(SwiftToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Swift enchant to Watering Can.</summary>
    [HarmonyPrefix]
    private static bool SwiftToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        __result = item is Tool tool && (tool is Axe or Hoe or Pickaxe ||
                                         (tool is WateringCan &&
                                          ToolsModule.Config.Can.AllowSwiftEnchantment));
        return false; // don't run original logic
    }

    #endregion harmony patches
}
