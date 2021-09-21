// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModChallenge : ModChallenge<TaikoHitObject>
    {
        public override HashSet<HitResult> JudgmentToggles => new HashSet<HitResult>() {
            HitResult.Ok,
            HitResult.Miss,
        };

        [SettingSource("Life Loss: OK or LOWER", "How many lives lost when obtaining an OK.")]
        public override Bindable<LivesLossPreset> LivesLossJ4 { get; } = new Bindable<LivesLossPreset>() { Default = LivesLossPreset.One, Value = LivesLossPreset.One, };
        [SettingSource("Life Loss: MISS", "How many lives lost when MISSing a note.")]
        public override Bindable<LivesLossPreset> LivesLossJ1 { get; } = new Bindable<LivesLossPreset>() { Default = LivesLossPreset.None, Value = LivesLossPreset.None, };
    }
}
