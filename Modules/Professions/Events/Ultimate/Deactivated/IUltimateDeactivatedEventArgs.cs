﻿namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate.Deactivated;

/// <summary>Interface for the arguments of an <see cref="UltimateDeactivatedEvent"/>.</summary>
public interface IUltimateDeactivatedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
