﻿namespace DaLion.Redux.Framework.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Framework.Professions.Ultimates;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="DustSpiritBehaviorAtGameTickPatch"/> class.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        this.Target = this.RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Ultimate.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || __instance.Player.Get_Ultimate() is not
                Ambush { IsActive: true })
        {
            return;
        }

        ___seenFarmer = false;
    }

    #endregion harmony patches
}
