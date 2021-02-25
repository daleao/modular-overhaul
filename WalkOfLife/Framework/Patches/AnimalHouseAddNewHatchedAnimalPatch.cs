﻿using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal AnimalHouseAddNewHatchedAnimalPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
				transpiler: new HarmonyMethod(GetType(), nameof(AnimalHouseAddNewHatchedAnimalTranspiler))
			);
		}

		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		protected static IEnumerable<CodeInstruction> AnimalHouseAddNewHatchedAnimalTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(AnimalHouse)}::{nameof(AnimalHouse.addNewHatchedAnimal)}.");

			/// Injected (twice): if (Game1.player.professions.Contains(<breeder_id>) a.friendshipTowardFarmer = Game1.random.Next(0, 500)

			Label isNotBreeder1 = iLGenerator.DefineLabel();
			Label isNotBreeder2 = iLGenerator.DefineLabel();
			int i = 0;
			repeat:
			try
			{
				_helper
					.Find(												// find the index of setting newborn display name
						fromCurrentIndex: i == 0 ? false : true,
						new CodeInstruction(OpCodes.Callvirt, operand: AccessTools.Property(typeof(FarmAnimal), nameof(FarmAnimal.displayName)).GetSetMethod())
					)
					.Advance()
					.AddLabel(i == 0 ? isNotBreeder1 : isNotBreeder2)	// the destination if player is not breeder
					.Retreat()
					.InsertProfessionCheck(ProfessionsMap.Forward["breeder"], branchDestination: i == 0 ? isNotBreeder1 : isNotBreeder2)
					.Insert(
						// load the field FarmAnimal.friendshipTowardFarmer
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(FarmAnimal)} (5)"),	// local 5 = FarmAnimal a
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.friendshipTowardFarmer))),

						// load the field Game1.random and roll a random int between 0 and 500
						new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.random))),
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 500),
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Random), nameof(Random.Next))),

						// set it to FarmerAnimal.friendshipTowardFarmer
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(NetFieldBase<Int32, NetInt>), nameof(NetFieldBase<Int32, NetInt>.Value)).GetSetMethod())
					);
					
			}
			catch (Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Breeder animal pregnancy chance.\nHelper returned {ex}");
			}

			// repeat injection (first iteration for coop animals, second for barn animals)
			if (++i < 2)
				goto repeat;

			return _helper.Flush();
		}
	}
}