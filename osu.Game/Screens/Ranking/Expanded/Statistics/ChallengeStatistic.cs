// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Ranking.Expanded.Accuracy;
using osu.Game.Scoring;
using osuTK;
using osu.Framework.Logging;

namespace osu.Game.Screens.Ranking.Expanded.Statistics
{
    /// <summary>
    /// A <see cref="StatisticDisplay"/> to display the player's combo.
    /// </summary>
    public class ChallengeStatistic : CounterStatistic
    {
        /// <summary>
        /// Creates a new <see cref="ChallengeStatistic"/>.
        /// </summary>
        public ChallengeStatistic(ScoreInfo score, ModChallenge modData)
            : base("LIFE", 0)
        {
            if (modData.LivesLeft != null) {
                count = modData.LivesLeft.Value;
                maxCount = modData.LivesLeft.MaxValue;
            } else {
                maxCount = modData.LivesAmount.Value;
                count = modData.LivesAmount.Value;
                int penaltyRate = 0;
                if (modData.LivesLossJ5 != null)
                    penaltyRate += (int)modData.LivesLossJ5.Value;
                count -= score.Statistics[HitResult.Great] * penaltyRate;
                if (modData.LivesLossJ4 != null)
                    penaltyRate += (int)modData.LivesLossJ4.Value;
                count -= score.Statistics[HitResult.Good] * penaltyRate;
                if (modData.LivesLossJ3 != null)
                    penaltyRate += (int)modData.LivesLossJ3.Value;
                count -= score.Statistics[HitResult.Ok] * penaltyRate;
                if (modData.LivesLossJ2 != null)
                    penaltyRate += (int)modData.LivesLossJ2.Value;
                count -= score.Statistics[HitResult.Meh] * penaltyRate;
                if (modData.LivesLossJ1 != null)
                    penaltyRate += (int)modData.LivesLossJ1.Value;
                count -= score.Statistics[HitResult.Miss] * penaltyRate;
                Logger.Log($"Challenge Mod Result {score.StatisticsJson}", LoggingTarget.Runtime, LogLevel.Verbose);
                Logger.Log($"Challenge Mod Result {modData.LivesLossJ5}/{modData.LivesLossJ4}/{modData.LivesLossJ3}/{modData.LivesLossJ2}/{modData.LivesLossJ1}", LoggingTarget.Runtime, LogLevel.Verbose);
            }
        }

        public override void Appear()
        {
            base.Appear();

            /* if (isPerfect)
            {
                using (BeginDelayedSequence(AccuracyCircle.ACCURACY_TRANSFORM_DURATION / 2))
                    perfectText.FadeIn(50);
            } */
        }

        // protected override Drawable CreateContent() => new FillFlowContainer
        // {
        //     AutoSizeAxes = Axes.Both,
        //     Direction = FillDirection.Horizontal,
        //     Spacing = new Vector2(10, 0),
        //     Children = new[]
        //     {
        //         base.CreateContent().With(d =>
        //         {
        //             Anchor = Anchor.CentreLeft;
        //             Origin = Anchor.CentreLeft;
        //         }),
        //         perfectText = new OsuSpriteText
        //         {
        //             Anchor = Anchor.CentreLeft,
        //             Origin = Anchor.CentreLeft,
        //             Text = "PERFECT",
        //             Font = OsuFont.Torus.With(size: 11, weight: FontWeight.SemiBold),
        //             Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex("#66FFCC"), Color4Extensions.FromHex("#FF9AD7")),
        //             Alpha = 0,
        //             UseFullGlyphHeight = false,
        //         }
        //     }
        // };
    }
}
