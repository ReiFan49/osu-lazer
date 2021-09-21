// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Audio;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osu.Game.Screens.Ranking.Expanded.Statistics;
using osuTK.Graphics;

using osu.Framework.Logging;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModChallenge : ModFailCondition
    {
        public override string Name => "Challenge";
        public override string Acronym => "CT";
        public override IconUsage? Icon => FontAwesome.Solid.HeartBroken;
        public override ModType Type => ModType.DifficultyIncrease;
        public override string Description => "Limited amount of mistakes.";
        public override double ScoreMultiplier => 1;
        public override bool RequiresConfiguration => true;

        [SettingSource("Number of Lives", "The number of lives you start with.")]
        public BindableInt LivesAmount { get; } = new BindableInt(10)
        {
            MinValue = 2,
            MaxValue = 999,
            Precision = 5,
        };

        [JsonIgnore]
        public virtual HashSet<HitResult> JudgmentToggles { get; }
        public enum LivesLossPreset {
            None = 0,
            [Description("1 LIFE")] One = 1,
            [Description("2 LIFE")] Two = 2,
            [Description("3 LIFE")] Three = 3,
            [Description("4 LIFE")] Four = 4,
            [Description("5 LIFE")] Five = 5,
            [Description("10 LIFE")] Ten = 10,
            [Description("99 LIFE")] AlmostHundred = 99,
        }

        // NOTE: J5 - Great, J1 - Miss
        // LivesLoss adds up from previous tier
        public virtual Bindable<LivesLossPreset> LivesLossJ5 { get; }
        public virtual Bindable<LivesLossPreset> LivesLossJ4 { get; }
        public virtual Bindable<LivesLossPreset> LivesLossJ3 { get; }
        public virtual Bindable<LivesLossPreset> LivesLossJ2 { get; }
        public virtual Bindable<LivesLossPreset> LivesLossJ1 { get; }

        public BindableInt LivesLeft;
        protected SkinnableSound comboBreakSample;

        public override Type[] IncompatibleMods => base.IncompatibleMods
            .Append(typeof(ModSuddenDeath))
            .Append(typeof(ModPerfect))
            .ToArray();
        
        public override string SettingDescription {
            get {
                List<string> desc = new List<string>();
                List<int> livesRule = new List<int>();

                desc.Add($"{LivesAmount.Value} LIFE");
                int totalPenalty = 0;
                if (LivesLossJ5 != null) { totalPenalty -= (int)LivesLossJ5.Value; livesRule.Add(totalPenalty); }
                if (LivesLossJ4 != null) { totalPenalty -= (int)LivesLossJ4.Value; livesRule.Add(totalPenalty); }
                if (LivesLossJ3 != null) { totalPenalty -= (int)LivesLossJ3.Value; livesRule.Add(totalPenalty); }
                if (LivesLossJ2 != null) { totalPenalty -= (int)LivesLossJ2.Value; livesRule.Add(totalPenalty); }
                if (LivesLossJ1 != null) { totalPenalty -= (int)LivesLossJ1.Value; livesRule.Add(totalPenalty); }
                if (livesRule.Any() && livesRule.Min() < 0)
                    desc.Add("Rule " + string.Join('/', livesRule));
                
                return string.Join(", ", desc);
            }
        }

        protected ModChallenge()
        {
            Restart.Value = Restart.Default = false;
        }

        protected override bool FailCondition(HealthProcessor healthProcessor, JudgementResult result)
        {
            // ignore indirect judgments.
            if (!(result.Type > HitResult.None && result.Type <= HitResult.Perfect))
                return false;

            if (!JudgmentToggles.Contains(result.Type))
                return false;

            int currentValue = LivesLeft.Value;
            int penaltyValue = 0;

            if (result.Type <= HitResult.Great && this.LivesLossJ5 != null)
                    penaltyValue += (int)this.LivesLossJ5.Value;
            if (result.Type <= HitResult.Good && this.LivesLossJ4 != null)
                    penaltyValue += (int)this.LivesLossJ4.Value;
            if (result.Type <= HitResult.Ok && this.LivesLossJ3 != null)
                    penaltyValue += (int)this.LivesLossJ3.Value;
            if (result.Type <= HitResult.Meh && this.LivesLossJ2 != null)
                    penaltyValue += (int)this.LivesLossJ2.Value;
            if (result.Type <= HitResult.Miss && this.LivesLossJ1 != null)
                    penaltyValue += (int)this.LivesLossJ1.Value;

            currentValue = currentValue - penaltyValue;
            if (penaltyValue > 0 && comboBreakSample != null)
                comboBreakSample.Play();
            Logger.Log($"Challenge Mod Status -> LOSS {penaltyValue} LIFE, CURRENT {currentValue} LIFE", LoggingTarget.Runtime, LogLevel.Verbose);
            if (currentValue < 0) currentValue = 0;
            LivesLeft.Value = currentValue;

            return LivesLeft.Value <= 0;
        }
    }

    public abstract class ModChallenge<T>  : ModChallenge, IApplicableToDrawableRuleset<T>
        where T : HitObject
    {
        public virtual void ApplyToDrawableRuleset(DrawableRuleset<T> drawableRuleset)
        {
            comboBreakSample = new SkinnableSound(new SampleInfo("Gameplay/combobreak"));
            Logger.Log($"Challenge Mod Status -> STARTING FROM {this.LivesAmount} WITH {this.LivesLossJ5}/{this.LivesLossJ4}/{this.LivesLossJ3}/{this.LivesLossJ2}/{this.LivesLossJ1}", LoggingTarget.Runtime, LogLevel.Verbose);

            int maxLives = (int)LivesAmount.Value;
            LivesLeft = new BindableInt()
            {
                Value = maxLives,
                MaxValue = maxLives,
            };

            // drawableRuleset.Playfield;
            // (drawableRuleset.Playfield as SentakkiPlayfield).AccentContainer.Add(new LiveCounter(LivesLeft));
        }

        public class LiveDrawCounter : RollingCounter<int> {
            public LiveDrawCounter() {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
            }
            protected override OsuSpriteText CreateSpriteText() {
                return new OsuSpriteText {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = OsuFont.Torus.With(size: 40, weight: FontWeight.SemiBold),
                    ShadowColour = Color4.Gray,
                };
            }
        }
        /* */
    }


}
