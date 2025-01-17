// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Input.Bindings;

namespace osu.Game.Skinning.Editor
{
    /// <summary>
    /// A container which handles loading a skin editor on user request for a specified target.
    /// This also handles the scaling / positioning adjustment of the target.
    /// </summary>
    public class SkinEditorOverlay : CompositeDrawable, IKeyBindingHandler<GlobalAction>
    {
        private readonly ScalingContainer target;
        private SkinEditor skinEditor;

        public const float VISIBLE_TARGET_SCALE = 0.8f;

        [Resolved]
        private OsuColour colours { get; set; }

        public SkinEditorOverlay(ScalingContainer target)
        {
            this.target = target;
            RelativeSizeAxes = Axes.Both;
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.Back:
                    if (skinEditor?.State.Value != Visibility.Visible)
                        break;

                    Hide();
                    return true;

                case GlobalAction.ToggleSkinEditor:
                    Toggle();
                    return true;
            }

            return false;
        }

        public void Toggle()
        {
            if (skinEditor == null)
                Show();
            else
                skinEditor.ToggleVisibility();
        }

        public override void Hide()
        {
            // base call intentionally omitted.
            skinEditor.Hide();
        }

        public override void Show()
        {
            // base call intentionally omitted.
            if (skinEditor == null)
            {
                LoadComponentAsync(skinEditor = new SkinEditor(target), AddInternal);
                skinEditor.State.BindValueChanged(editorVisibilityChanged);
            }
            else
                skinEditor.Show();
        }

        private void editorVisibilityChanged(ValueChangedEvent<Visibility> visibility)
        {
            if (visibility.NewValue == Visibility.Visible)
            {
                updateMasking();
                target.AllowScaling = false;
                target.RelativePositionAxes = Axes.Both;

                target.ScaleTo(VISIBLE_TARGET_SCALE, SkinEditor.TRANSITION_DURATION, Easing.OutQuint);
                target.MoveToX(0.095f, SkinEditor.TRANSITION_DURATION, Easing.OutQuint);
            }
            else
            {
                target.AllowScaling = true;

                target.ScaleTo(1, SkinEditor.TRANSITION_DURATION, Easing.OutQuint).OnComplete(_ => updateMasking());
                target.MoveToX(0f, SkinEditor.TRANSITION_DURATION, Easing.OutQuint);
            }
        }

        private void updateMasking() =>
            target.Masking = skinEditor.State.Value == Visibility.Visible;

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        /// <summary>
        /// Exit any existing skin editor due to the game state changing.
        /// </summary>
        public void Reset()
        {
            skinEditor?.Save();
            skinEditor?.Hide();
            skinEditor?.Expire();

            skinEditor = null;
        }
    }
}
