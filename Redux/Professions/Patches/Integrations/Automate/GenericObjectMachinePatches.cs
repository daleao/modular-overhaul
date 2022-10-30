﻿namespace DaLion.Redux.Professions.Patches.Integrations;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Core.Extensions;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
internal sealed class GenericObjectMachinePatches : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GenericObjectMachinePatches"/> class.</summary>
    internal GenericObjectMachinePatches()
    {
        this.Transpiler!.after = new[] { ReduxModule.Tweex.Name };
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.ApplyImpl(harmony);
        }
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1"
            .ToType()
            .MakeGenericType(typeof(SObject))
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .First(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3);

        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine"
            .ToType()
            .RequireMethod("SetInput");

        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine"
            .ToType()
            .RequireMethod("SetInput");
    }

    #region harmony patches

    /// <summary>Patch to apply Artisan effects to automated generic machines.</summary>
    [HarmonyTranspiler]
    [HarmonyAfter("DaLion.Redux.Tweex")]
    private static IEnumerable<CodeInstruction>? GenericObjectMachineTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: ApplyArtisanPerks(this.Machine, this.Location, consumable.Sample)
        // Before: return true;
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine`1"
                            .ToType()
                            .MakeGenericType(typeof(SObject))
                            .RequirePropertyGetter("Machine")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine"
                            .ToType()
                            .RequirePropertyGetter("Location")),
                    new CodeInstruction(original.DeclaringType!.Name.Contains("Loom")
                        ? OpCodes.Ldloc_1
                        : OpCodes.Ldloc_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        "Pathoschild.Stardew.Automate.IConsumable"
                            .ToType()
                            .RequirePropertyGetter("Sample")),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GenericObjectMachinePatches).RequireMethod(nameof(ApplyArtisanPerks))));
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching modded Artisan behavior for generic Automate machines." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void ApplyArtisanPerks(SObject machine, GameLocation location, Item sample)
    {
        if (!machine.IsArtisanMachine() || !machine.heldObject.Value.IsArtisanGood() ||
            sample is not SObject input)
        {
            return;
        }

        var output = machine.heldObject.Value;
        var chest = ExtendedAutomateApi.GetClosestContainerTo(machine, location);
        var user = ModEntry.Config.Professions.LaxOwnershipRequirements ? Game1.player : chest?.GetOwner() ?? Game1.MasterPlayer;
        if (user.HasProfession(Profession.Artisan) ||
            (ModEntry.Config.Professions.LaxOwnershipRequirements && Game1.game1.DoesAnyPlayerHaveProfession(Profession.Artisan, out _)))
        {
            output.Quality = input.Quality;
        }

        var owner = ModEntry.Config.Professions.LaxOwnershipRequirements ? Game1.player : machine.GetOwner();
        if (!owner.HasProfession(Profession.Artisan))
        {
            return;
        }

        if (output.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
        {
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
        }

        if (owner.HasProfession(Profession.Artisan, true))
        {
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 4;
        }
        else
        {
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;
        }

        if (machine.ParentSheetIndex == (int)Machine.MayonnaiseMachine && input.ParentSheetIndex == Constants.GoldenEggIndex &&
            !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod"))
        {
            output.Quality = SObject.bestQuality;
        }
    }

    #endregion injected subroutines
}