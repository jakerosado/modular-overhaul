﻿namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class DemolitionistUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId;

    /// <summary>Initializes a new instance of the <see cref="DemolitionistUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DemolitionistUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        this._buffId = (ModEntry.Manifest.UniqueID + Profession.Demolitionist).GetHashCode();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var excitedness = Game1.player.Get_DemolitionistExcitedness();
        if (excitedness <= 0)
        {
            this.Disable();
        }

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == this._buffId);
        if (buff is not null)
        {
            return;
        }

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
                excitedness,
                0,
                0,
                1,
                "Demolitionist",
                ModEntry.i18n.Get(
                    "demolitionist.title" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = this._buffId,
                sheetIndex = Profession.DemolitionistExcitedSheetIndex,
                millisecondsDuration = 555,
                description = ModEntry.i18n.Get("demolitionist.buff.desc"),
            });

        var decay = excitedness >= 4 ? 2 : 1;
        Game1.player.Decrement_DemolitionistExcitedness(decay);
    }
}
