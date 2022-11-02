﻿namespace DaLion.Redux.Framework.Professions.Patches.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHitWithHoePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CropHitWithHoePatch"/> class.</summary>
    internal CropHitWithHoePatch()
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.hitWithHoe));
    }

    #region harmony patches

    /// <summary>Apply Botanist/Ecologist perk to wild ginger.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CropHitWithHoeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: SetGingerQuality(obj);
        // Between: obj = new SObject(829, 1);
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(new CodeInstruction(OpCodes.Stloc_0))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertProfessionCheck(Profession.Ecologist)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while apply Ecologist/Botanist perk to hoed ginger.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
