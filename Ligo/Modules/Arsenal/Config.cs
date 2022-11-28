﻿namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Configs;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Arsenal.</summary>
public sealed class Config
{
    /// <inheritdoc cref="SlingshotConfig"/>
    [JsonProperty]
    public SlingshotConfig Slingshots { get; internal set; } = new();

    /// <inheritdoc cref="WeaponConfig"/>
    [JsonProperty]
    public WeaponConfig Weapons { get; internal set; } = new();

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your arsenal.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when using weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat.</summary>
    [JsonProperty]
    public bool OverhauledKnockback { get; internal set; } = true;

    /// <summary>Gets increases the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 2f;

    /// <summary>Gets increases the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1.2f;

    /// <summary>Gets increases the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool AncientCrafting { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool InfinityPlusOne { get; internal set; } = true;
}
