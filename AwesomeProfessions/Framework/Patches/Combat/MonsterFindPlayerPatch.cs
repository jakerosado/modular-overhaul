﻿using DaLion.Stardew.Professions.Framework.SuperMode;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MonsterFindPlayerPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterFindPlayerPatch()
    {
        Original = RequireMethod<Monster>("findPlayer");
        Prefix.before = new[] {"Esca.FarmTypeManager"};
    }

    #region harmony patches

    /// <summary>Patch to override monster aggro.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("Esca.FarmTypeManager")]
    private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer __result)
    {
        try
        {
            __result = Game1.player;
            if (!Context.IsMultiplayer || __instance.currentLocation is null)
                return false; // don't run original logic

            // Slimes prefer Pipers
            if (__instance is GreenSlime &&
                __instance.currentLocation.DoesAnyPlayerHereHaveProfession(Profession.Piper, out var pipers))
            {
                var distanceToClosestPiper = double.MaxValue;
                foreach (var piper in pipers)
                {
                    var distanceToThisPiper = __instance.DistanceToCharacter(piper);
                    if (distanceToThisPiper >= distanceToClosestPiper) continue;

                    __result = piper;
                    distanceToClosestPiper = distanceToThisPiper;
                }

                return false; // don't run original logic
            }

            // prefer Brutes
            //if (__instance.currentLocation.DoesAnyPlayerHereHaveProfession("Brute", out var brutes))
            //{
            //	var distanceToClosestBrute = double.MaxValue;
            //	foreach (var brute in brutes)
            //	{
            //		var distanceToThisBrute = __instance.DistanceToCharacter(brute);
            //		if (distanceToThisBrute >= distanceToClosestBrute) continue;

            //		__result = brute;
            //		distanceToClosestBrute = distanceToThisBrute;

            //	}

            //	return false; // don't run original logic
            //}

            // avoid Poachers in Ambuscade
            var distanceToClosestPlayer = double.MaxValue;
            foreach (var farmer in __instance.currentLocation.farmers)
            {
                if (ModEntry.State.Value.ActivePeerSuperModes.TryGetValue(SuperModeIndex.Poacher,
                        out var peerIds) && peerIds.Any(id => id == farmer.UniqueMultiplayerID)) continue;

                var distanceToThisPlayer = __instance.DistanceToCharacter(farmer);
                if (distanceToThisPlayer >= distanceToClosestPlayer) continue;

                __result = farmer;
                distanceToClosestPlayer = distanceToThisPlayer;
            }

            //if (__result.IsLocalPlayer && ModEntry.State.Value.IsSuperModeActive &&
            //    ModEntry.State.Value.SuperModeIndex == Util.Professions.IndexOf("Poacher"))
            //	__result = null;

            return false; // run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}