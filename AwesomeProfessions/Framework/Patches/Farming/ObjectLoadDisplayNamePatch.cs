﻿namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectLoadDisplayNamePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectLoadDisplayNamePatch()
    {
        Original = RequireMethod<SObject>("loadDisplayName");
    }

    /// <summary>Patch to add honey-specific mead names.</summary>
    [HarmonyPostfix]
    private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
    {
        if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0) return;

        var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
        __result = prefix + ' ' + __result;
    }
}