﻿namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnLeaveLocationPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingOnLeaveLocationPatch"/> class.</summary>
    internal RingOnLeaveLocationPatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onLeaveLocation));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnLeaveLocationPrefix(Ring __instance)
    {
        return !ModEntry.Config.Rings.TheOneInfinityBand ||
               __instance.indexInTileSheet.Value != Constants.IridiumBandIndex;
    }

    #endregion harmony patches
}
