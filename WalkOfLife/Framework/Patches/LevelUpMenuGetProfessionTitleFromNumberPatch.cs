﻿using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		private static ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch(IMonitor monitor, ITranslationHelper i18n)
		: base(monitor)
		{
			_i18n = i18n;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getProfessionTitleFromNumber)),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to apply modded profession names.</summary>
		protected static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			if (!Globals.ProfessionMap.Contains(whichProfession)) return true; // run original logic

			__result = _i18n.Get(Globals.ProfessionMap.Reverse[whichProfession] + ".name");
			return false; // don't run original logic
		}
		#endregion harmony patches
	}

}
