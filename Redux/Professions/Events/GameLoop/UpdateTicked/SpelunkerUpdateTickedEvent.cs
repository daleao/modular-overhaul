﻿namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerUpdateTickedEvent : UpdateTickedEvent
{
    private const int BuffSheetIndex = 40;

    private readonly int _buffId = (ModEntry.Manifest.UniqueID + Profession.Spelunker).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="SpelunkerUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is not MineShaft)
        {
            return;
        }

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == this._buffId);
        if (buff is not null)
        {
            return;
        }

        var bonusLadderChance = (ModEntry.State.Professions.SpelunkerLadderStreak * 0.5f).ToString("0.0");
        var bonusSpeed = Math.Min(
            (ModEntry.State.Professions.SpelunkerLadderStreak / 10) + 1,
            (int)ModEntry.Config.Professions.SpelunkerSpeedCap);
        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                bonusSpeed,
                0,
                0,
                1,
                "Spelunker",
                ModEntry.i18n.Get("spelunker.title" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = this._buffId,
                sheetIndex = BuffSheetIndex,
                millisecondsDuration = 0,
                description =
                    ModEntry.i18n.Get("spelunker.buff.desc", new { bonusLadderChance, bonusSpeed }),
            });
    }
}