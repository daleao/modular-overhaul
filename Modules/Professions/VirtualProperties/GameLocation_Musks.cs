﻿namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Musks
{
    internal static ConditionalWeakTable<GameLocation, HashSet<Musk>> Values { get; } = new();

    // returns nullable to avoid unnecessary iteration
    internal static IEnumerable<Musk> Get_Musks(this GameLocation location)
    {
        return Values.TryGetValue(location, out var musks) ? musks : Enumerable.Empty<Musk>();
    }

    internal static void AddMusk(this GameLocation location, Musk musk)
    {
        Values.GetOrCreateValue(location).Add(musk);
    }

    internal static void AddMusk(this GameLocation location, Vector2 position, int duration)
    {
        Values.GetOrCreateValue(location).Add(new Musk(location, position, duration));
    }

    internal static void RemoveMusk(this GameLocation location, Musk musk)
    {
        Values.GetOrCreateValue(location).Remove(musk);
    }
}
