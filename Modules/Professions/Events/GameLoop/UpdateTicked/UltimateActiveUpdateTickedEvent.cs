﻿namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateActiveUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateActiveUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var ultimate = Game1.player.Get_Ultimate();
        Game1.player.startGlowing(ultimate!.GlowColor, false, 0.05f);
        if (Game1.game1.ShouldTimePass())
        {
            ultimate.Countdown();
        }
    }
}
