﻿namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

using DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCouldInventoryAcceptThisObjectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCouldInventoryAcceptThisObjectPatcher"/> class.</summary>
    internal FarmerCouldInventoryAcceptThisObjectPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.couldInventoryAcceptThisObject));
    }

    #region harmony patches

    /// <summary>Allow picking up blueprints.</summary>
    [HarmonyPrefix]
    private static bool FarmerCouldInventoryAcceptThisObjectPrefix(ref bool __result, int index)
    {
        if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue ||
            JsonAssetsIntegration.DwarvishBlueprintIndex.Value != index)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
