﻿namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntStarted;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntEnded;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntStartedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntType Type { get; }

    /// <summary>Gets the coordinates of the target tile.</summary>
    Vector2 Target { get; }
}
