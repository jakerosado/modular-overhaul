﻿namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Ligo.Modules.Arsenal.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Dark Sword.</summary>
[XmlType("Mods_DaLion_CursedEnchantment")]
public class CursedEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (who.Read<bool>(DataFields.Cursed))
        {
            return;
        }

        who.Write(DataFields.Cursed, true.ToString());
        ModEntry.Events.Enable<CurseUpdateTickedEvent>();
        Log.D($"{Game1.player.Name} has been cursed!");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        who.health = Math.Max(who.health - (int)(who.maxHealth * 0.01f), 0);
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        base._OnMonsterSlay(m, location, who);
        if (who.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex } darkSword)
        {
            return;
        }

        darkSword.Increment(DataFields.CursePoints, 20);
        darkSword.minDamage.Value += 1;
        darkSword.maxDamage.Value += 1;
    }
}
