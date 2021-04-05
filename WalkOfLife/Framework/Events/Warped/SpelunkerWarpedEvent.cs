﻿using StardewModdingAPI.Events;
using StardewValley.Locations;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && e.NewLocation is MineShaft)
			{
				uint currentMineLevel = (uint)(e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", uint.Parse))
					AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", currentMineLevel.ToString());
			}
		}
	}
}