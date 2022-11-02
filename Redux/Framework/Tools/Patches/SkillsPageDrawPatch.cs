﻿namespace DaLion.Redux.Framework.Tools.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatch"/> class.</summary>
    internal SkillsPageDrawPatch()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Allows new Master Enchantments to draw as green levels in the skills page.</summary>
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);
        var currentTool = generator.DeclareLocal(typeof(Tool));

        try
        {
            var checkForMasterEnchantment = generator.DefineLabel();
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Cgt),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Stloc_S, currentTool),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(OpCodes.Isinst, typeof(Hoe)),
                    new CodeInstruction(OpCodes.Brtrue_S, checkForMasterEnchantment),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(OpCodes.Isinst, typeof(WateringCan)),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse))
                .InsertWithLabels(
                    new[] { checkForMasterEnchantment },
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(MasterEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse))
                .InsertWithLabels(
                    new[] { setTrue },
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { setFalse },
                    new CodeInstruction(OpCodes.Ldc_I4_0));
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Farming Level for new Master enchantments.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Cgt),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Stloc_S, currentTool),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(MasterEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse))
                .InsertWithLabels(
                    new[] { setTrue },
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { setFalse },
                    new CodeInstruction(OpCodes.Ldc_I4_0));
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Mining Level for new Master enchantment.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Cgt),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Stloc_S, currentTool),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(MasterEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, setFalse))
                .InsertWithLabels(
                    new[] { setTrue },
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { setFalse },
                    new CodeInstruction(OpCodes.Ldc_I4_0));
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Foraging Level for new Master enchantment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }
}
