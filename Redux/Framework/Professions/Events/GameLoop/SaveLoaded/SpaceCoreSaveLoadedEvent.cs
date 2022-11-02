﻿namespace DaLion.Redux.Framework.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
[AlwaysEnabled]
internal sealed class SpaceCoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpaceCoreSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        // get custom luck skill
        if (Framework.Integrations.LuckSkillApi is not null)
        {
            var luckSkill = new LuckSkill(Framework.Integrations.LuckSkillApi);
            SCSkill.Loaded["spacechase0.LuckSkill"] = luckSkill;
            foreach (var profession in luckSkill.Professions)
            {
                SCProfession.LoadedProfessions[profession.Id] = (SCProfession)profession;
            }
        }

        // get remaining SpaceCore skills
        foreach (var skillId in Framework.Integrations.SpaceCoreApi!.GetCustomSkills())
        {
            var customSkill = new SCSkill(skillId);
            SCSkill.Loaded[skillId] = customSkill;
            foreach (var profession in customSkill.Professions)
            {
                SCProfession.LoadedProfessions[profession.Id] = (SCProfession)profession;
            }
        }

        // revalidate levels
        SCSkill.Loaded.Values.ForEach(s => s.Revalidate());
    }
}
