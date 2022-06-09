﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Localisation;

namespace osu.Game.Overlays.Settings.Sections.Graphics
{
    public class DetailSettings : SettingsSubsection
    {
        protected override LocalisableString Header => GraphicsSettingsStrings.DetailSettingsHeader;

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = GraphicsSettingsStrings.StoryboardVideo,
                    Current = config.GetBindable<bool>(OsuSetting.ShowStoryboard)
                },
                new SettingsCheckbox
                {
                    LabelText = GraphicsSettingsStrings.HitLighting,
                    Current = config.GetBindable<bool>(OsuSetting.HitLighting)
                },
                new SettingsEnumDropdown<ScreenshotFormat>
                {
                    LabelText = GraphicsSettingsStrings.ScreenshotFormat,
                    Current = config.GetBindable<ScreenshotFormat>(OsuSetting.ScreenshotFormat)
                },
                new SettingsCheckbox
                {
                    LabelText = GraphicsSettingsStrings.ShowCursorInScreenshots,
                    Current = config.GetBindable<bool>(OsuSetting.ScreenshotCaptureMenuCursor)
                }
            };
        }
    }
}