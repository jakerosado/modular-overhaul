﻿namespace DaLion.Ligo;

#region using directives

using DaLion.Shared.UI;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Holds global variables that may be used by different modules.</summary>
internal sealed class Globals
{
    /// <summary>Gets the <see cref="HudPointer"/> which points at various points of interest.</summary>
    internal static Lazy<HudPointer> Pointer { get; } = new(() => new HudPointer(
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/HudPointer"),
        ModEntry.Config.Professions.TrackPointerScale,
        ModEntry.Config.Professions.TrackPointerBobbingRate));

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet gemstone (provided by Json Assets).</summary>
    internal static int? GarnetIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet Ring (provided by Json Assets).</summary>
    internal static int? GarnetRingIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Infinity Band (provided by Json Assets).</summary>
    internal static int? InfinityBandIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Hero Soul (provided by Dynamic Game Assets).</summary>
    internal static int? HeroSoulindex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Dwarven Scrap (provided by Dynamic Game Assets).</summary>
    internal static int? DwarvenScrapIndex { get; set; }

    /// <summary>Gets or sets the <see cref="FrameRateCounter"/>.</summary>
    internal static FrameRateCounter? FpsCounter { get; set; }

    /// <summary>Gets or sets the latest position of the cursor.</summary>
    internal static ICursorPosition? DebugCursorPosition { get; set; }
}
