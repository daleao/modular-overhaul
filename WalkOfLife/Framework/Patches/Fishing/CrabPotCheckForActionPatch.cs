﻿using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CrabPotCheckForActionPatch : BasePatch
	{
		//private static IReflectionHelper _reflection;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CrabPotCheckForActionPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction)),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotCheckForActionPrefix))
			);
		}

		/// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
		protected static bool CrabPotCheckForActionPrefix(ref CrabPot __instance, ref bool __result, ref bool ___lidFlapping, ref float ___lidFlapTimer, ref Vector2 ___shake, ref float ___shakeTimer, Farmer who, bool justCheckingForActivity = false)
		{
			if (__instance.tileIndexToShow != 714 || justCheckingForActivity || !_IsFishButNotTrapFish(__instance.heldObject.Value))
			{
				return true; // run original logic
			}

			SObject item = __instance.heldObject.Value;
			__instance.heldObject.Value = null;
			if (who.IsLocalPlayer && !who.addItemToInventoryBool(item))
			{
				__instance.heldObject.Value = item;
				Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
				__result = false;
				return false; // don't run original logic;
			}

			Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
			if (data.ContainsKey(item.ParentSheetIndex))
			{
				string[] rawData = data[item.ParentSheetIndex].Split('/');
				int minFishSize = Convert.ToInt32(rawData[3]);
				int maxFishSize = Convert.ToInt32(rawData[4]);
				who.caughtFish(item.ParentSheetIndex, Game1.random.Next(minFishSize, maxFishSize + 1));
			}

			__instance.readyForHarvest.Value = false;
			__instance.tileIndexToShow = 710;
			___lidFlapping = true;
			___lidFlapTimer = 60f;
			__instance.bait.Value = null;
			who.animateOnce(279 + who.FacingDirection);
			who.currentLocation.playSound("fishingRodBend");
			DelayedAction.playSoundAfterDelay("coin", 500);
			who.gainExperience(1, 5);
			___shake = Vector2.Zero;
			___shakeTimer = 0f;
			
			__result = true;
			return false; // don't run original logic
		}

		/// <summary>Whether a given object is a fish that's not a regular crab pot fish.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsFishButNotTrapFish(SObject obj)
		{
			return obj?.Type == "Fish" && !(_IsTrapFish(obj) || _IsTrash(obj));
		}

		/// <summary>Whether a given object is a crab pot fish.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsTrapFish(SObject obj)
		{
			return obj?.ParentSheetIndex > 714 && obj?.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a trash.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsTrash(SObject obj)
		{
			return obj?.ParentSheetIndex > 166 && obj?.ParentSheetIndex < 173;
		}
	}
}