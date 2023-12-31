﻿namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.UiInfoSuite;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Annosz.UiInfoSuite2", version: "2.2.6")]
internal sealed class ExperienceBarDrawExperienceBarPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ExperienceBarDrawExperienceBarPatcher"/> class.</summary>
    internal ExperienceBarDrawExperienceBarPatcher()
    {
        this.Target = "UIInfoSuite2.UIElements.ExperienceBar"
            .ToType()
            .RequireMethod("DrawExperienceBar");
    }

    #region harmony patches

    /// <summary>Patch to move skill icon to the right.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ExperienceBarDrawExperienceBarTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 54f, ...
        // To: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 162f, ...
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R4, 54f) })
                .SetOperand(174f);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed to budge Ui Info Suite experience bar skill icon." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
