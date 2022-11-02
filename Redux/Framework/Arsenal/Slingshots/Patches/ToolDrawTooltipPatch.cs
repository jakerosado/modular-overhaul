﻿namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawTooltipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawTooltipPatch"/> class.</summary>
    internal ToolDrawTooltipPatch()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw Slingshot enchantment effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawTooltipPrefix(
        Tool __instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        if (__instance is not Slingshot slingshot || slingshot.enchantments.Count == 0)
        {
            return true; // run original logic
        }

        // write description
        ItemDrawTooltipPatch.ItemDrawTooltipReverse(__instance, spriteBatch, ref x, ref y, font, alpha, overrideText);

        Color co;

        // write bonus damage
        if (__instance.hasEnchantmentOfType<RubyEnchantment>())
        {
            var amount =
                $"+{(__instance.GetEnchantmentLevel<RubyEnchantment>() * 0.1f) + __instance.Read<float>(DataFields.ResonantSlingshotDamage):0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(120, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get("ui.itemhover.damage", new { amount }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (__instance.hasEnchantmentOfType<AquamarineEnchantment>())
        {
            var amount =
                $"+{(__instance.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f) + __instance.Read<float>(DataFields.ResonantSlingshotCritChance):0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(40, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (__instance.hasEnchantmentOfType<JadeEnchantment>())
        {
            var amount =
                $"+{(__instance.GetEnchantmentLevel<JadeEnchantment>() * (ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f)) + __instance.Read<float>(DataFields.ResonantSlingshotCritPower):0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 16, y + 16 + 4),
                new Rectangle(160, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount),
                font,
                new Vector2(x + 16 + 44, y + 16 + 12),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        if (__instance.hasEnchantmentOfType<AmethystEnchantment>())
        {
            var amount =
                $"+{(__instance.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f) + __instance.Read<float>(DataFields.ResonantSlingshotKnockback)}:0%";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(70, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_Weight", amount),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus fire speed
        if (__instance.hasEnchantmentOfType<EmeraldEnchantment>())
        {
            var amount =
                $"+{(__instance.GetEnchantmentLevel<EmeraldEnchantment>() + __instance.Read<float>(DataFields.ResonantSlingshotSpeed)) * 0.1f:0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(130, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get("ui.itemhover.firespeed", new { amount }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus cooldown reduction
        if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
        {
            var amount =
                $"-{(__instance.GetEnchantmentLevel<GarnetEnchantment>() + __instance.Read<float>(DataFields.ResonantSlingshotCooldownReduction)) * 0.1f:0%}";
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(150, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.i18n.Get("ui.itemhover.cdr", new { amount }),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (__instance.hasEnchantmentOfType<TopazEnchantment>())
        {
            var amount = ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.OverhauledDefense
                ? $"+{(__instance.GetEnchantmentLevel<TopazEnchantment>() + __instance.Read<float>(DataFields.ResonantSlingshotDefense)) * 0.1f:0%}"
                : __instance.GetEnchantmentLevel<TopazEnchantment>().ToString();
            co = new Color(0, 120, 120);
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors,
                new Vector2(x + 20, y + 20),
                new Rectangle(110, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.OverhauledDefense
                    ? ModEntry.i18n.Get("ui.itemhover.resist", new { amount })
                    : Game1.content.LoadString("ItemHover_DefenseBonus", amount),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus random forge
        if (__instance.enchantments.Count > 0 && __instance.enchantments[^1] is DiamondEnchantment)
        {
            co = new Color(0, 120, 120);
            var randomForges = __instance.GetMaxForges() - __instance.GetTotalForgeLevels();
            var randomForgeString = randomForges != 1
                ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", randomForges)
                : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", randomForges);
            Utility.drawTextWithShadow(
                spriteBatch,
                randomForgeString,
                font,
                new Vector2(x + 16, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write other enchantments
        co = new Color(120, 0, 210);
        foreach (var enchantment in __instance.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed()))
        {
            Utility.drawWithShadow(
                spriteBatch,
                Game1.mouseCursors2,
                new Vector2(x + 20, y + 20),
                new Rectangle(127, 35, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                false,
                1f);
            Utility.drawTextWithShadow(
                spriteBatch,
                BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(),
                font,
                new Vector2(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // extra height to compensate `Forged` text
        if (__instance.enchantments.Count > 0)
        {
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f) / 4;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
