﻿using System.Collections.Generic;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties related to objects.</summary>
	public static partial class Utility
	{
		#region look-up tables

		/// <summary>Look-up table for what resource should spawn from a given stone.</summary>
		public static Dictionary<int, int> ResourceFromStoneId { get; } = new Dictionary<int, int>
		{
			// stone
			{ 668, 390 },
			{ 670, 390 },
			{ 845, 390 },
			{ 846, 390 },
			{ 847, 390 },

			// ores
			{ 751, 378 },
			{ 849, 378 },
			{ 290, 380 },
			{ 850, 380 },
			{ 764, 384 },
			{ 765, 386 },
			{ 95, 909 },

			// geodes
			{ 75, 535 },
			{ 76, 536 },
			{ 77, 537 },
			{ 819, 749 },

			// gems
			{ 8, 66 },
			{ 10, 68 },
			{ 12, 60 },
			{ 14, 62 },
			{ 6, 70 },
			{ 4, 64 },
			{ 2, 72 },

			// other
			{ 25, 719 },
			{ 816, 881 },
			{ 817, 881 },
			{ 818, 330 },
			{ 843, 848 },
			{ 844, 848 }
		};

		/// <summary>Set of id's corresponding to animal produce or derived artisan goods.</summary>
		private static IEnumerable<int> _AnimalProductIds { get; } = new HashSet<int>
		{
			107,	// dinosaur egg
			174,	// large egg
			176,	// egg
			180,	// brown egg
			182,	// large brown egg
			184,	// milk
			186,	// large milk
			289,	// ostrich egg
			305,	// void egg
			306,	// mayonnaise
			307,	// duck mayonnaise
			308,	// void mayonnaise
			424,	// cheese
			426,	// goat cheese
			428,	// cloth
			436,	// goat milk
			438,	// large goat milk
			440,	// wool
			442,	// duck egg
			444,	// duck feather
			446,	// rabbit's foot
			807,	// dinosaur mayonnaise
			928		// golden egg
		};

		/// <summary>Set of id's corresponding to legendary fish.</summary>
		private static IEnumerable<int> _LegendaryFishIds { get; } = new HashSet<int>
		{
			159,	// crimsonfish
			160,	// angler
			163,	// legend
			682,	// mutant carp
			775,	// glacierfish
			898,	// son of crimsonfish
			899,	// ms. angler
			900,	// legend ii
			901,	// radioactive carp
			902		// glacierfish jr.
		};

		/// <summary>Set of item id's corresponding to gems or minerals.</summary>
		private static IEnumerable<int> _GemIds { get; } = new HashSet<int>
		{
			SObject.emeraldIndex,
			SObject.aquamarineIndex,
			SObject.rubyIndex,
			SObject.amethystClusterIndex,
			SObject.topazIndex,
			SObject.sapphireIndex,
			SObject.diamondIndex,
			SObject.prismaticShardIndex
		};

		/// <summary>Set of ammunition id's.</summary>
		private static IEnumerable<int> _MineralAmmunitionIds { get; } = new HashSet<int>
		{
			SObject.copper + 1,
			SObject.iron + 1,
			SObject.coal + 1,
			SObject.gold + 1,
			SObject.iridium + 1,
			SObject.stone + 1,
		};

		/// <summary>Set of id's corresponding to stones that should be trackable.</summary>
		private static IEnumerable<int> _ResourceNodeIds { get; } = new HashSet<int>
		{
			// ores
			751,	// copper node
			849,	// copper ?
			290,	// silver node
			850,	// silver ?
			764,	// gold node
			765,	// iridium node
			95,		// radioactive node

			// geodes
			75,		// geode node
			76,		// frozen geode node
			77,		// magma geode node
			819,	// omni geode node

			// gems
			8,		// amethyst node
			10,		// topaz node
			12,		// emerald node
			14,		// aquamarine node
			6,		// jade node
			4,		// ruby node
			2,		// diamond node
			44,		// gem node

			// other
			25,		// mussel node
			816,	// bone node
			817,	// bone node
			818,	// clay node
			843,	// cinder shard node
			844,	// cinder shard node
			46		// mystic stone
		};

		#endregion look-up tables

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsAnimalProduct(SObject obj)
		{
			return obj != null && _AnimalProductIds.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsWildBerry(SObject obj)
		{
			return obj?.ParentSheetIndex == 296 || obj?.ParentSheetIndex == 410;
		}

		/// <summary>Whether a given object is a spring onion.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsSpringOnion(SObject obj)
		{
			return obj?.ParentSheetIndex == 399;
		}

		/// <summary>Whether a given object is one of wine, juice, beer, mead or pale ale.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsBeverage(SObject obj)
		{
			int pale_ale = 303, beer = 346, wine = 348, juice = 350, mead = 459, green_tea = 614;
			return obj != null && (obj.Name.Contains("Wine") || obj.Name.Contains("Juice") || obj.Name.Contains("Mead") || obj.ParentSheetIndex.AnyOf(beer, green_tea, juice, mead, pale_ale, wine));
		}

		/// <summary>Whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		public static bool IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}

		/// <summary>Whether a givenitem index corresponds to a gem or mineral.</summary>
		/// <param name="index">The item index.</param>
		public static bool IsMineralIndex(int index)
		{
			return _GemIds.Contains(index) || (index > 537 && index < 579);
		}

		/// <summary>Whether a given object is a foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsForagedMineral(SObject obj)
		{
			return obj.Name.AnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
		}

		/// <summary>Whether a given object is a resource node or foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsResourceNode(SObject obj)
		{
			return IsStone(obj) && _ResourceNodeIds.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is a crab pot fish.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsTrapFish(SObject obj)
		{
			return obj?.ParentSheetIndex > 714 && obj?.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsReeledFish(SObject obj)
		{
			return obj.Category == SObject.FishCategory && !IsTrapFish(obj);
		}

		/// <summary>Whether a given object is a trash.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsTrash(SObject obj)
		{
			return obj?.ParentSheetIndex > 166 && obj?.ParentSheetIndex < 173;
		}

		/// <summary>Whether a given item index corresponds to trash.</summary>
		/// <param name="index">An item index.</param>
		public static bool IsTrash(int index)
		{
			return index > 166 && index < 173;
		}

		/// <summary>Whether a given item index corresponds to mineral ammunition.</summary>
		/// <param name="index">An item index.</param>
		public static bool IsMineralAmmunition(int index)
		{
			return _MineralAmmunitionIds.Contains(index);
		}
	}
}