﻿namespace DaLion.Stardew.Professions.Framework.TreasureHunts;

#region using directives

using Common.Data;
using Common.Multiplayer;
using Events.Display;
using Events.GameLoop;
using Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Manages treasure hunt events for Scavenger professions.</summary>
internal sealed class ScavengerHunt : TreasureHunt
{
    private readonly int[] _artifactsThatCanBeFound =
    {
        100, // chipped amphora
        101, // arrowhead
        103, // ancient doll
        104, // elvish jewelry
        105, // chewing stick
        106, // ornamental fan
        109, // ancient sword
        114, // ancient seed
        115, // prehistoric tool
        118, // glass shards
        119, // bone flute
        120, // prehistoric hand-axe
        123, // ancient drum
        124, // golden mask
        125, // golden relic
        126, // strange doll
        127, // strange doll
    };

    /// <summary>Construct an instance.</summary>
    internal ScavengerHunt()
    {
        huntStartedMessage = ModEntry.i18n.Get("scavenger.huntstarted");
        huntFailedMessage = ModEntry.i18n.Get("scavenger.huntfailed");
        iconSourceRect = new(80, 656, 16, 16);
    }

    #region public methods

    /// <inheritdoc />
    public override bool TryStart(GameLocation location)
    {
        if (!TryStart()) return false;

        TreasureTile = ChooseTreasureTile(location);
        if (TreasureTile is null) return false;

        huntLocation = location;
        huntLocation.MakeTileDiggable(TreasureTile.Value);
        timeLimit = (uint)(location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                            ModEntry.Config.ScavengerHuntHandicap);
        timeLimit = Math.Max(timeLimit, 30);

        elapsed = 0;
        ModEntry.EventManager.Hook<PointerUpdateTickedEvent>();
        ModEntry.EventManager.Hook<ScavengerHuntRenderedHudEvent>();
        ModEntry.EventManager.Hook<ScavengerHuntUpdateTickedEvent>();
        Game1.addHUDMessage(new HuntNotification(huntStartedMessage, iconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Scavenger, true))
            {
                if (!Context.IsMainPlayer)
                    ModEntry.Broadcaster.Message("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                else
                    ModEntry.EventManager.Hook<HostPrestigeTreasureHuntUpdateTickedEvent>();
            }
        }

        OnStarted();
        return true;
    }

    /// <inheritdoc />
    public override void ForceStart(GameLocation location, Vector2 target)
    {
        ForceStart();

        TreasureTile = target;
        huntLocation = location;
        huntLocation.MakeTileDiggable(TreasureTile.Value);
        timeLimit = (uint)(location.Map.DisplaySize.Area / Math.Pow(Game1.tileSize, 2) / 100 *
                            ModEntry.Config.ScavengerHuntHandicap);
        timeLimit = Math.Max(timeLimit, 30);

        elapsed = 0;
        ModEntry.EventManager.Hook<PointerUpdateTickedEvent>();
        ModEntry.EventManager.Hook<ScavengerHuntRenderedHudEvent>();
        ModEntry.EventManager.Hook<ScavengerHuntUpdateTickedEvent>();
        Game1.addHUDMessage(new HuntNotification(huntStartedMessage, iconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Scavenger, true))
            {
                if (!Context.IsMainPlayer)
                {
                    ModEntry.Broadcaster.Message("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                }
                else
                {
                    ModEntry.EventManager.Hook<HostPrestigeTreasureHuntUpdateTickedEvent>();
                    ModEntry.HostState.PlayersHuntingTreasure.Add(Game1.player.UniqueMultiplayerID);
                }
            }
        }

        OnStarted();
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(huntFailedMessage));
        ModDataIO.WriteTo(Game1.player, "ScavengerHuntStreak", "0");
        End(false);
    }

    #endregion public methods

    #region protected methods

    /// <inheritdoc />
    protected override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 v;
        var failsafe = 0;
        do
        {
            if (failsafe > 69) return null;

            var x = random.Next(location.Map.DisplayWidth / Game1.tileSize);
            var y = random.Next(location.Map.DisplayHeight / Game1.tileSize);
            v = new(x, y);
            ++failsafe;
        } while (!location.IsTileValidForTreasure(v));

        return v;

        //var candidates = Tiles.FloodFill(Game1.player.getTileLocation(), location.Map.DisplayWidth / Game1.tileSize,
        //    location.Map.DisplayHeight / Game1.tileSize, location.IsTileValidForTreasure);
        //if (candidates.Count > 0) return candidates.ElementAt(random.Next(candidates.Count));

        //return null;
    }

    /// <inheritdoc />
    protected override void CheckForCompletion()
    {
        if (TreasureTile is null || !huntLocation.terrainFeatures.TryGetValue(TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
            return;

        var getTreasure = new DelayedAction(200, BeginFindTreasure);
        Game1.delayedActions.Add(getTreasure);
        ModDataIO.Increment<uint>(Game1.player, "ScavengerHuntStreak");
        End(true);
    }

    /// <inheritdoc />
    protected override void End(bool found)
    {
        ModEntry.EventManager.Unhook<ScavengerHuntRenderedHudEvent>();
        ModEntry.EventManager.Unhook<ScavengerHuntUpdateTickedEvent>();
        TreasureTile = null;
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(Profession.Scavenger, true)) return;

        Broadcaster.SendPublicChat(found
            ? $"{Game1.player.Name} has found the treasure!"
            : $"{Game1.player.Name} failed to find the treasure.");
        ModEntry.Broadcaster.Message("HuntIsOff", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);

        OnEnded(found);
    }

    #endregion protected methods

    #region private methods

    /// <summary>Play treasure chest found animation.</summary>
    private void BeginFindTreasure()
    {
        Game1.currentLocation.TemporarySprites.Add(new(
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"), new(64, 1920, 32, 32), 500f, 1, 0,
            Game1.player.Position + new Vector2(-32f, -160f), false, false,
            Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
        {
            motion = new(0f, -0.128f),
            timeBasedMotion = true,
            endFunction = OpenChestEndFunction,
            extraInfoForEndBehavior = 0,
            alpha = 0f,
            alphaFade = -0.002f
        });
    }

    /// <summary>Play open treasure chest animation.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenChestEndFunction(int extra)
    {
        Game1.currentLocation.localSound("openChest");
        Game1.currentLocation.TemporarySprites.Add(new(
            PathUtilities.NormalizeAssetName("LooseSprites/Cursors"), new(64, 1920, 32, 32), 200f, 4, 0,
            Game1.player.Position + new Vector2(-32f, -228f), false, false,
            Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
        {
            endFunction = OpenTreasureMenuEndFunction,
            extraInfoForEndBehavior = 0
        });
    }

    /// <summary>Open the treasure chest menu.</summary>
    /// <param name="extra">Not applicable.</param>
    private void OpenTreasureMenuEndFunction(int extra)
    {
        Game1.player.completelyStopAnimatingOrDoingAction();
        var treasures = GetTreasureContents();
        Game1.activeClickableMenu = new ItemGrabMenu(treasures).setEssential(true);
        ((ItemGrabMenu)Game1.activeClickableMenu).source = ItemGrabMenu.source_fishingChest;
    }

    /// <summary>Choose the contents of the treasure chest.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private List<Item> GetTreasureContents()
    {
        List<Item> treasures = new();
        var chance = 1.0;
        while (random.NextDouble() <= chance)
        {
            chance *= 0.4f;
            if (Game1.currentSeason == "spring" && !(Game1.currentLocation is Beach) && random.NextDouble() < 0.1)
                treasures.Add(new SObject(273,
                    random.Next(2, 6) + (random.NextDouble() < 0.25 ? 5 : 0))); // rice shoot

            if (random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                treasures.Add(new SObject(890,
                    random.Next(1, 3) + (random.NextDouble() < 0.25 ? 2 : 0))); // qi beans

            switch (random.Next(4))
            {
                case 0:
                    {
                        List<int> possibles = new();
                        if (random.NextDouble() < 0.4) possibles.Add(386); // iridium ore

                        if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(384); // gold ore

                        if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(380); // iron ore

                        if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(378); // copper ore

                        if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(388); // wood

                        if (possibles.Count == 0 || random.NextDouble() < 0.4) possibles.Add(390); // stone

                        possibles.Add(382); // coal
                        treasures.Add(new SObject(possibles.ElementAt(random.Next(possibles.Count)),
                            random.Next(2, 7) *
                            (!(random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.015) ? 1 : 2)));
                        if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03) treasures.Last().Stack *= 2;

                        break;
                    }
                case 1:
                    {
                        if (random.NextDouble() < 0.25 && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
                            treasures.Add(new SObject(774, 5 + (random.NextDouble() < 0.25 ? 5 : 0))); // wild bait
                        else
                            treasures.Add(new SObject(685, 10)); // bait

                        break;
                    }
                case 2:
                    {
                        if (random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound.Value < 21 &&
                            Game1.player.hasOrWillReceiveMail("lostBookFound"))
                            treasures.Add(new SObject(102, 1)); // lost book
                        else if (Game1.player.archaeologyFound.Any() && random.NextDouble() < 0.5) // artifacts
                            treasures.Add(new SObject(random.NextDouble() < 0.5
                                ? _artifactsThatCanBeFound[random.Next(_artifactsThatCanBeFound.Length)]
                                : random.NextDouble() < 0.25
                                    ? 114
                                    : 535, 1));
                        else
                            treasures.Add(new SObject(382, random.Next(1, 3))); // coal

                        break;
                    }
                case 3:
                    {
                        switch (random.Next(3))
                        {
                            case 0:
                                treasures.Add(new SObject(random.Next(535, 538), random.Next(1, 4))); // geodes
                                if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
                                    treasures.Last().Stack *= 2;

                                break;

                            case 1:
                                switch (random.Next(4))
                                {
                                    case 0: // fire quartz else ruby or emerald
                                        treasures.Add(new SObject(
                                            random.NextDouble() < 0.3 ? 82 : random.NextDouble() < 0.5 ? 64 : 60,
                                            random.Next(1, 3)));
                                        break;

                                    case 1: // frozen tear else jade or aquamarine
                                        treasures.Add(new SObject(
                                            random.NextDouble() < 0.3 ? 84 : random.NextDouble() < 0.5 ? 70 : 62,
                                            random.Next(1, 3)));
                                        break;

                                    case 2: // earth crystal else amethyst or topaz
                                        treasures.Add(new SObject(
                                            random.NextDouble() < 0.3 ? 86 : random.NextDouble() < 0.5 ? 66 : 68,
                                            random.Next(1, 3)));
                                        break;

                                    case 3:
                                        treasures.Add(random.NextDouble() < 0.28
                                            ? new(72, 1) // diamond
                                            : new SObject(80, random.Next(1, 3))); // quartz
                                        break;
                                }

                                if (random.NextDouble() < 0.05) treasures.Last().Stack *= 2;

                                break;

                            case 2:
                                var luckModifier = 1.0 + Game1.player.DailyLuck * 10;
                                var streak =
                                    ModDataIO.ReadFrom<uint>(Game1.player, "ScavengerHuntStreak");
                                if (random.NextDouble() < 0.025 * luckModifier &&
                                    !Game1.player.specialItems.Contains(15))
                                    treasures.Add(new MeleeWeapon(15) { specialItem = true }); // forest sword

                                if (random.NextDouble() < 0.025 * luckModifier &&
                                    !Game1.player.specialItems.Contains(20))
                                    treasures.Add(new MeleeWeapon(20) { specialItem = true }); // elf blade

                                if (random.NextDouble() < 0.07 * luckModifier)
                                    switch (random.Next(3))
                                    {
                                        case 0:
                                            treasures.Add(new Ring(516 +
                                                                   (random.NextDouble() < Game1.player.LuckLevel / 11f
                                                                       ? 1
                                                                       : 0))); // (small) glow ring
                                            break;

                                        case 1:
                                            treasures.Add(new Ring(518 +
                                                                   (random.NextDouble() < Game1.player.LuckLevel / 11f
                                                                       ? 1
                                                                       : 0))); // (small) magnet ring
                                            break;

                                        case 2:
                                            treasures.Add(new Ring(random.Next(529, 535))); // gemstone ring
                                            break;
                                    }

                                if (random.NextDouble() < 0.02 * luckModifier)
                                    treasures.Add(new SObject(166, 1)); // treasure chest

                                if (random.NextDouble() < 0.005 * luckModifier * Math.Pow(2, streak))
                                    treasures.Add(new SObject(74, 1)); // prismatic shard

                                if (random.NextDouble() < 0.01 * luckModifier)
                                    treasures.Add(new SObject(126, 1)); // strange doll

                                if (random.NextDouble() < 0.01 * luckModifier)
                                    treasures.Add(new SObject(127, 1)); // strange doll

                                if (random.NextDouble() < 0.01 * luckModifier)
                                    treasures.Add(new Ring(527)); // iridium band

                                if (random.NextDouble() < 0.01 * luckModifier)
                                    treasures.Add(new Boots(random.Next(504, 514))); // boots

                                if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") &&
                                    random.NextDouble() < 0.01 * luckModifier)
                                    treasures.Add(new SObject(928, 1)); // golden egg

                                if (treasures.Count == 1) treasures.Add(new SObject(72, 1)); // consolation diamond

                                break;
                        }

                        break;
                    }
            }
        }

        if (random.NextDouble() < 0.4)
            switch (Game1.currentSeason) // forage seeds
            {
                case "spring":
                    treasures.Add(new SObject(495, 1));
                    break;

                case "summer":
                    treasures.Add(new SObject(496, 1));
                    break;

                case "fall":
                    treasures.Add(new SObject(497, 1));
                    break;

                case "winter":
                    treasures.Add(new SObject(498, 1));
                    break;
            }
        else
            treasures.Add(new SObject(770, random.Next(1, 4) * 5)); // wild seeds

        return treasures;
    }

    #endregion private methods
}