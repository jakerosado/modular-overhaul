﻿namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetNumberOfDescriptionCategoriesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetNumberOfDescriptionCategoriesPatcher"/> class.</summary>
    internal MeleeWeaponGetNumberOfDescriptionCategoriesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getNumberOfDescriptionCategories));
    }

    #region harmony patches

    /// <summary>Correct number of description categories.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetNumberOfDescriptionCategoriesPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            __result = __instance.CountNonZeroStats();
            if (__instance.enchantments.Count > 0 && __instance.enchantments[^1] is DiamondEnchantment)
            {
                __result++;
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
