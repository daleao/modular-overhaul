﻿namespace DaLion.Overhaul.Modules.Ponds.Extensions;

#region using directives

using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
internal static class RandomExtensions
{
    /// <summary>Gets the item index of a random algae.</summary>
    /// <param name="random">The <see cref="Random"/> number generator.</param>
    /// <param name="bias">A particular type of algae that should be favored.</param>
    /// <returns>The <see cref="int"/> index of an algae <see cref="Item"/>.</returns>
    internal static int NextAlgae(this Random random, int? bias = null)
    {
        if (bias.HasValue && random.NextDouble() > 2d / 3d)
        {
            return bias.Value;
        }

        return random.NextDouble() switch
        {
            > 2d / 3d => ObjectIds.GreenAlgae,
            > 1d / 3d => ObjectIds.Seaweed,
            _ => ObjectIds.WhiteAlgae,
        };
    }
}
