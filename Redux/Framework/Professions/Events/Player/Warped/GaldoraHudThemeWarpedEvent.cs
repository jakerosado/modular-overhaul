﻿namespace DaLion.Redux.Framework.Professions.Events.Player;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Integration("FlashShifter.StardewValleyExpandedCP")]
[AlwaysEnabled]
internal sealed class GaldoraHudThemeWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GaldoraHudThemeWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal GaldoraHudThemeWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.NameOrUniqueName.IsIn(
                "Custom_CastleVillageOutpost",
                "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry",
                "Custom_TreasureCave"))
        {
            ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }
    }
}
