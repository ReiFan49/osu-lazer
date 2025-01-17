// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Screens;
using osu.Game.Screens.Menu;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual
{
    /// <summary>
    /// A scene which tests full game flow.
    /// </summary>
    public abstract class OsuGameTestScene : OsuManualInputManagerTestScene
    {
        protected TestOsuGame Game;

        protected override bool UseFreshStoragePerRun => true;

        protected override bool CreateNestedActionContainer => false;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black,
            };
        }

        [SetUpSteps]
        public virtual void SetUpSteps()
        {
            AddStep("Create new game instance", () =>
            {
                if (Game?.Parent != null)
                {
                    Remove(Game);
                    Game.Dispose();
                }

                RecycleLocalStorage(false);

                CreateGame();
            });

            AddUntilStep("Wait for load", () => Game.IsLoaded);
            AddUntilStep("Wait for intro", () => Game.ScreenStack.CurrentScreen is IntroScreen);

            ConfirmAtMainMenu();
        }

        [TearDownSteps]
        public void TearDownSteps()
        {
            AddStep("exit game", () => Game.Exit());
            AddUntilStep("wait for game exit", () => Game.Parent == null);
        }

        protected void CreateGame()
        {
            AddGame(Game = new TestOsuGame(LocalStorage, API));
        }

        protected void PushAndConfirm(Func<Screen> newScreen)
        {
            Screen screen = null;
            AddStep("Push new screen", () => Game.ScreenStack.Push(screen = newScreen()));
            AddUntilStep("Wait for new screen", () => Game.ScreenStack.CurrentScreen == screen && screen.IsLoaded);
        }

        protected void ConfirmAtMainMenu() => AddUntilStep("Wait for main menu", () => Game.ScreenStack.CurrentScreen is MainMenu menu && menu.IsLoaded);

        public class TestOsuGame : OsuGame
        {
            public new const float SIDE_OVERLAY_OFFSET_RATIO = OsuGame.SIDE_OVERLAY_OFFSET_RATIO;

            public new ScreenStack ScreenStack => base.ScreenStack;

            public new BackButton BackButton => base.BackButton;

            public new BeatmapManager BeatmapManager => base.BeatmapManager;

            public new ScoreManager ScoreManager => base.ScoreManager;

            public new Container ScreenOffsetContainer => base.ScreenOffsetContainer;

            public new SettingsOverlay Settings => base.Settings;

            public new NotificationOverlay Notifications => base.Notifications;

            public new MusicController MusicController => base.MusicController;

            public new OsuConfigManager LocalConfig => base.LocalConfig;

            public new Bindable<WorkingBeatmap> Beatmap => base.Beatmap;

            public new Bindable<RulesetInfo> Ruleset => base.Ruleset;

            public new Bindable<IReadOnlyList<Mod>> SelectedMods => base.SelectedMods;

            // if we don't do this, when running under nUnit the version that gets populated is that of nUnit.
            public override string Version => "test game";

            protected override Loader CreateLoader() => new TestLoader();

            public new void PerformFromScreen(Action<IScreen> action, IEnumerable<Type> validScreens = null) => base.PerformFromScreen(action, validScreens);

            public TestOsuGame(Storage storage, IAPIProvider api)
            {
                Storage = storage;
                API = api;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                API.Login("Rhythm Champion", "osu!");

                Dependencies.Get<SessionStatics>().SetValue(Static.MutedAudioNotificationShownOnce, true);
            }
        }

        public class TestLoader : Loader
        {
            protected override ShaderPrecompiler CreateShaderPrecompiler() => new TestShaderPrecompiler();

            private class TestShaderPrecompiler : ShaderPrecompiler
            {
                protected override bool AllLoaded => true;
            }
        }
    }
}
