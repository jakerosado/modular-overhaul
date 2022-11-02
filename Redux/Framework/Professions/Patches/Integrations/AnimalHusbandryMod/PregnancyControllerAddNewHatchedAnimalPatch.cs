﻿namespace DaLion.Redux.Framework.Professions.Patches.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("DIGUS.ANIMALHUSBANDRYMOD")]
internal sealed class PregnancyControllerAddNewHatchedAnimalPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="PregnancyControllerAddNewHatchedAnimalPatch"/> class.</summary>
    internal PregnancyControllerAddNewHatchedAnimalPatch()
    {
        this.Target = "AnimalHusbandryMod.animals.PregnancyController"
            .ToType()
            .RequireMethod("addNewHatchedAnimal");
    }

    #region harmony patches

    /// <summary>Patch for Rancher husbanded animals to have random starting friendship.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PregnancyControllerAddNewHatchedAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: AddNewHatchedAnimalSubroutine(farmAnimal);
        // Before: AnimalHouse animalHouse = building.indoors.Value as AnimalHouse;
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Building).RequireField(nameof(Building.indoors))))
                .RetreatUntil(new CodeInstruction(OpCodes.Nop))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(PregnancyControllerAddNewHatchedAnimalPatch)
                            .RequireMethod(nameof(AddNewHatchedAnimalSubroutine))));
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching Rancher husbanded newborn friendship." +
                  "\n—-- Do NOT report this to Animal Husbandry's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void AddNewHatchedAnimalSubroutine(FarmAnimal newborn)
    {
        var owner = Game1.getFarmer(newborn.ownerID.Value);
        if (!owner.HasProfession(Profession.Rancher))
        {
            return;
        }

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(newborn.myID.GetHashCode()).Next(-50, 51);
    }

    #endregion private methods
}
