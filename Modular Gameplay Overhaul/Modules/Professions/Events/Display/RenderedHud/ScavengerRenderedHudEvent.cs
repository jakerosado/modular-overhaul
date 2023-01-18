﻿namespace DaLion.Overhaul.Modules.Professions.Events.Display;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerRenderedHudEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (ProfessionsModule.Config.DisableAlwaysTrack && !ProfessionsModule.Config.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = ProfessionsModule.Config.ModKey.IsDown();

        // track objects
        foreach (var (key, @object) in Game1.currentLocation.Objects.Pairs)
        {
            if (!@object.ShouldBeTrackedBy(Profession.Scavenger))
            {
                continue;
            }

            Globals.Pointer.Value.DrawAsTrackingPointer(key, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(key, Color.Yellow);
            }
        }

        //track berries
        foreach (var feature in Game1.currentLocation.largeTerrainFeatures)
        {
            if (feature is not Bush bush || bush.townBush.Value || bush.tileSheetOffset.Value != 1 ||
                !bush.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth))
            {
                continue;
            }

            Globals.Pointer.Value.DrawAsTrackingPointer(bush.tilePosition.Value, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(bush.tilePosition.Value + new Vector2(0.5f, -1f), Color.Yellow);
            }
        }

        // track ginger
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not HoeDirt { crop: { } crop } dirt || !crop.forageCrop.Value)
            {
                continue;
            }

            Globals.Pointer.Value.DrawAsTrackingPointer(dirt.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(dirt.currentTileLocation, Color.Yellow);
            }
        }

        // track coconuts
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not Tree tree || !tree.hasSeed.Value || tree.treeType.Value != Tree.palmTree)
            {
                continue;
            }

            Globals.Pointer.Value.DrawAsTrackingPointer(tree.currentTileLocation, Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                Globals.Pointer.Value.DrawOverTile(tree.currentTileLocation, Color.Yellow);
            }
        }
    }
}
