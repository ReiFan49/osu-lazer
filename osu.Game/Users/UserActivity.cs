﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osuTK.Graphics;

namespace osu.Game.Users
{
    public abstract class UserActivity
    {
        public abstract string Status { get; }
        public virtual Color4 GetAppropriateColour(OsuColour colours) => colours.GreenDarker;

        public class Modding : UserActivity
        {
            public override string Status => "Modding a map";
            public override Color4 GetAppropriateColour(OsuColour colours) => colours.PurpleDark;
        }

        public class ChoosingBeatmap : UserActivity
        {
            public override string Status => "Choosing a beatmap";
        }

        public abstract class InGame : UserActivity
        {
            public BeatmapInfo Beatmap { get; }

            public RulesetInfo Ruleset { get; }

            protected InGame(BeatmapInfo info, RulesetInfo ruleset)
            {
                Beatmap = info;
                Ruleset = ruleset;
            }

            public override string Status => Ruleset.CreateInstance().PlayingVerb;
        }

        public class InMultiplayerGame : InGame
        {
            public InMultiplayerGame(BeatmapInfo beatmap, RulesetInfo ruleset)
                : base(beatmap, ruleset)
            {
            }

            public override string Status => $@"{base.Status} with others";
        }

        public class InPlaylistGame : InGame
        {
            public InPlaylistGame(BeatmapInfo beatmap, RulesetInfo ruleset)
                : base(beatmap, ruleset)
            {
            }
        }

        public class InSoloGame : InGame
        {
            public InSoloGame(BeatmapInfo info, RulesetInfo ruleset)
                : base(info, ruleset)
            {
            }
        }

        public class Editing : UserActivity
        {
            public BeatmapInfo Beatmap { get; }

            public Editing(BeatmapInfo info)
            {
                Beatmap = info;
            }

            public override string Status => @"Editing a beatmap";
        }

        public class Spectating : UserActivity
        {
            public override string Status => @"Spectating a game";
        }

        public class SearchingForLobby : UserActivity
        {
            public override string Status => @"Looking for a lobby";
        }

        public class InLobby : UserActivity
        {
            public override string Status => @"In a lobby";

            public readonly Room Room;

            public InLobby(Room room)
            {
                Room = room;
            }
        }
    }
}
