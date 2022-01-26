﻿namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using JetBrains.Annotations;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class BuffCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BuffCtorPatch()
    {
        Original = RequireConstructor<Buff>(typeof(int));
    }

    /// <summary>Patch to change Slimed debuff into Slimed buff for prestiged Piper.</summary>
    private static void BuffCtorPostfix(Buff __instance, int which)
    {
        if (which != 13 || !Game1.player.HasProfession(Profession.Piper, true)) return;
        __instance.buffAttributes[9] = -__instance.buffAttributes[9];
    }
}