﻿namespace DaLion.Ligo.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPagePerformHoverActionPatcher"/> class.</summary>
    internal SkillsPagePerformHoverActionPatcher()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    private static void SkillsPagePerformHoverActionPostfix(
        SkillsPage __instance, int x, int y, ref string ___hoverText)
    {
        ___hoverText = ___hoverText.Truncate(90);

        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        var bounds = ProfessionsModule.Config.PrestigeProgressionStyle switch
        {
            ProfessionsConfig.ProgressionStyle.StackedStars => new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Textures.ProgressionHorizontalOffset - 14,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset - 4,
                (int)(Textures.StarsWidth * Textures.StarsScale),
                (int)(Textures.StarsWidth * Textures.StarsScale)),
            ProfessionsConfig.ProgressionStyle.Gen3Ribbons => new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Textures.ProgressionHorizontalOffset,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset,
                (int)(Textures.RibbonWidth * Textures.RibbonScale),
                (int)(Textures.RibbonWidth * Textures.RibbonScale)),
            ProfessionsConfig.ProgressionStyle.Gen4Ribbons => new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Textures.ProgressionHorizontalOffset,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset,
                (int)(Textures.RibbonWidth * Textures.RibbonScale),
                (int)(Textures.RibbonWidth * Textures.RibbonScale)),
            _ => Rectangle.Empty,
        };

        for (var i = 0; i < 5; i++)
        {
            bounds.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };
            var professionsForThisSkill = Game1.player.GetProfessionsForSkill(skill, true);
            var count = professionsForThisSkill.Length;
            if (count == 0)
            {
                continue;
            }

            bounds.Width = ProfessionsModule.Config.PrestigeProgressionStyle is ProfessionsConfig.ProgressionStyle.Gen3Ribbons
                or ProfessionsConfig.ProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RibbonWidth * Textures.RibbonScale)
                : (int)(((Textures.SingleStarWidth / 2 * count) + 4) * Textures.StarsScale);
            if (!bounds.Contains(x, y))
            {
                continue;
            }

            ___hoverText = i18n.Get("prestige.skillpage.tooltip", new { count });
            ___hoverText = professionsForThisSkill
                .Select(p => p.Title)
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }

        if (Ligo.Integrations.SpaceCoreApi is null)
        {
            return;
        }

        foreach (var skill in SCSkill.Loaded.Values)
        {
            bounds.Y += 56;
            var professionsForThisSkill =
                Game1.player.GetProfessionsForSkill(skill, true);
            var count = professionsForThisSkill.Length;
            if (count == 0)
            {
                continue;
            }

            bounds.Width = ProfessionsModule.Config.PrestigeProgressionStyle is ProfessionsConfig.ProgressionStyle.Gen3Ribbons
                or ProfessionsConfig.ProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RibbonWidth * Textures.RibbonScale)
                : (int)(((Textures.SingleStarWidth / 2 * count) + 4) * Textures.StarsScale);
            if (!bounds.Contains(x, y))
            {
                continue;
            }

            ___hoverText = i18n.Get("prestige.skillpage.tooltip", new { count });
            ___hoverText = professionsForThisSkill
                .Select(p => p.Title)
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }
    }

    #endregion harmony patches
}
