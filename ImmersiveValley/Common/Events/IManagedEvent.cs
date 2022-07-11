﻿namespace DaLion.Common.Events;

/// <summary>Interface for an event wrapper allowing dynamic hooking / unhooking.</summary>
public interface IManagedEvent
{
    /// <summary>Whether this event is hooked.</summary>
    bool IsHooked { get; }

    /// <summary>Whether this event is hooked for a specific splitscreen player.</summary>
    /// <param name="screenID">The player's screen id.</param>
    bool IsHookedForScreen(int screenID);

    /// <summary>Hook this event on the current screen.</summary>
    /// <returns><see langword="true"> if the event's hooked status was changed, otherwise <see langword="false">.</returns>
    bool Hook();

    /// <summary>Unhook this event on the current screen.</summary>
    /// <returns><see langword="true"> if the event's hooked status was changed, otherwise <see langword="false">.</returns>
    bool Unhook();
}