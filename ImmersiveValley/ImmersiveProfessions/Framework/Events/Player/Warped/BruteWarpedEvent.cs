﻿namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Extensions;
using GameLoop;

#endregion using directives

[UsedImplicitly]
internal class BruteWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            EventManager.Enable(typeof(BruteUpdateTickedEvent));
        }
        else
        {
            ModEntry.PlayerState.BruteRageCounter = 0;
            EventManager.Disable(typeof(BruteUpdateTickedEvent));
        }
    }
}