﻿using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	internal class ConservationistDayEndingEvent : DayEndingEvent
	{
		/// <summary>Construct an instance.</summary>
		internal ConservationistDayEndingEvent() { }

		/// <summary>Raised before the game ends the current day. Receive Conservationist mail from the FRS about taxation bracket.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (Game1.dayOfMonth == 28 && _data.WaterTrashCollectedThisSeason > 0)
			{
				_data.ConservationistTaxBonusThisSeason = _data.WaterTrashCollectedThisSeason / _config.TrashNeededForNextTaxLevel / 100f;
				if (_data.ConservationistTaxBonusThisSeason > 0)
				{
					AwesomeProfessions.ModHelper.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow("ConservationistTaxNotice");
				}

				_data.WaterTrashCollectedThisSeason = 0;
			}
		}
	}
}
