﻿using Molten.Graphics;
using Molten.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Molten.Samples
{
    public abstract class SampleGame : Foundation
    {
        SpriteFont _sampleFont;
        bool _baseContentLoaded;
        ControlSampleForm _form;
        SceneLayer _spriteLayer;
        SceneLayer _uiLayer;
        CameraComponent _cam2D;


        public SampleGame(string title) : base(title) { }

        protected override void OnInitialize(Engine engine)
        {
            base.OnInitialize(engine);
            Window.OnHandleChanged += Window_OnHandleChanged;

            MainScene = CreateScene("Main");
            _spriteLayer = MainScene.AddLayer("sprite", true);
            _uiLayer = MainScene.AddLayer("ui", true);
            _uiLayer.BringToFront();

            // Use the same camera for both the sprite and UI scenes.
            _cam2D = MainScene.AddObjectWithComponent<CameraComponent>(_uiLayer);
            _cam2D.Mode = RenderCameraMode.Orthographic;
            _cam2D.OrderDepth = 1;
            _cam2D.MaxDrawDistance = 1.0f;
            _cam2D.OutputSurface = Window;
            _cam2D.LayerMask = BitwiseHelper.Set(_cam2D.LayerMask, 0);
            Engine.Input.Camera = _cam2D;

            ContentRequest cr = engine.Content.BeginRequest("assets/");
            cr.Load<SpriteFont>("BroshK.ttf;size=24");
            OnContentRequested(cr);
            cr.OnCompleted += Cr_OnCompleted;
            cr.Commit();
        }

        private void Window_OnHandleChanged(INativeSurface surface)
        {
            if (Settings.UseGuiControl && _form == null)
            {
                // Create form and find placeholder panel.
                _form = new ControlSampleForm();
                Control[] placeholderPanels = _form.Controls.Find("surfacePlaceholder", true);
                if (placeholderPanels.Length > 0)
                {
                    surface.ParentHandle = placeholderPanels[0].Handle;

                    // TODO remove winforms sliders once engine has own GUI system.
                    _form.SliderRed.ValueChanged += SliderRed_ValueChanged;
                    _form.SliderGreen.ValueChanged += SliderGreen_ValueChanged;
                    _form.SliderBlue.ValueChanged += SliderBlue_ValueChanged;
                    _form.Show();
                }
            }
        }

        private void SliderRed_ValueChanged(object sender, EventArgs e)
        {
            ChangePresentColor(0, (byte)((sender as TrackBar).Value));
        }

        private void SliderGreen_ValueChanged(object sender, EventArgs e)
        {
            ChangePresentColor(1, (byte)((sender as TrackBar).Value));
        }

        private void SliderBlue_ValueChanged(object sender, EventArgs e)
        {
            ChangePresentColor(2, (byte)((sender as TrackBar).Value));
        }

        private void ChangePresentColor(int channelIndex, byte val)
        {
            Color color = MainScene.BackgroundColor;
            color[channelIndex] = val;
            MainScene.BackgroundColor = color;
        }

        private void Cr_OnCompleted(ContentRequest cr)
        {
            _sampleFont = cr.Get<SpriteFont>(0);
            Engine.Renderer.Overlay.Font = _sampleFont;

            OnContentLoaded(cr);
            SampleSpriteRenderComponent com = _uiLayer.AddObjectWithComponent<SampleSpriteRenderComponent>();
            com.RenderCallback = OnHudDraw;
            com.DepthWriteOverride = GraphicsDepthWritePermission.Disabled;
            _baseContentLoaded = true;
        }

        protected virtual void OnContentRequested(ContentRequest cr) { }

        protected virtual void OnContentLoaded(ContentRequest cr) { }

        protected override void OnUpdate(Timing time)
        {
            // Don't update until the base content is loaded.
            if (!_baseContentLoaded)
                return;

            // Cycle through window modes.
            if (Keyboard.IsTapped(Key.F2))
            {
                switch (Window.Mode)
                {
                    case WindowMode.Borderless: Window.Mode = WindowMode.Windowed; break;
                    case WindowMode.Windowed: Window.Mode = WindowMode.Borderless; break;
                }
            }

            // Toggle overlay.
            if (Keyboard.IsTapped(Key.F1))
            {
                if (_cam2D.HasFlags(RenderCameraFlags.ShowOverlay))
                {
                    int cur = Engine.Renderer.Overlay.Current;

                    // Remove overlay flag if we've hit the last overlay.
                    if (!Engine.Renderer.Overlay.Next())
                    {
                        _cam2D.Flags &= ~RenderCameraFlags.ShowOverlay;
                        Engine.Renderer.Overlay.Current = 0;
                    }
                }
                else
                {
                    Engine.Renderer.Overlay.Current = 0;
                    _cam2D.Flags |= RenderCameraFlags.ShowOverlay;
                }
            }
        }

        protected virtual void OnHudDraw(SpriteBatcher sb)
        {
            string text = "[F1] debug overlay";
            Vector2F tSize = SampleFont.MeasureString(text);
            Vector2F pos = new Vector2F()
            {
                X = Window.Width / 2 + (-tSize.X / 2),
                Y = 5,
            };

            sb.DrawString(SampleFont, text, pos, Color.White);
        }

        public abstract string Description { get; }

        public SpriteFont SampleFont => _sampleFont;

        /// <summary>Gets a random number generator. Used for various samples.</summary>
        public Random Rng { get; private set; } = new Random();

        /// <summary>
        /// Gets the sample's UI scene layer.
        /// </summary>
        public SceneLayer UI => _uiLayer;

        /// <summary>
        /// Gets the sample's sprite scene layer.
        /// </summary>
        public SceneLayer SpriteLayer => _spriteLayer;

        /// <summary>
        /// Gets the sample's sprite scene. This is rendered before <see cref="UIScene"/>.
        /// </summary>
        public Scene MainScene { get; private set; }
    }
}
