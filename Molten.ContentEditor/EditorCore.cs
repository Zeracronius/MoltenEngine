using Molten;
using Molten.Font;
using Molten.Graphics;
using Molten.Input;
using Molten.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.ContentEditor
{
    public class EditorCore : Foundation<RendererDX11, WinInputManager>
    {
        Scene _uiScene;
        UIMenu _menu;
        UIPanel _leftPanel;
        UITextbox _textBox;

        internal EditorCore() : base("Molten Editor")
        {

        }

        protected override void OnInitialize(Engine engine)
        {
            base.OnInitialize(engine);

            Scene MainScene = CreateScene("Main");
            SceneLayer _spriteLayer = MainScene.AddLayer("sprite", true);
            SceneLayer _uiLayer = MainScene.AddLayer("ui", true);
            _uiLayer.BringToFront();

            // Use the same camera for both the sprite and UI scenes.
            CameraComponent _cam2D = MainScene.AddObjectWithComponent<CameraComponent>(_uiLayer);
            _cam2D.Mode = RenderCameraMode.Orthographic;
            _cam2D.OrderDepth = 1;
            _cam2D.MaxDrawDistance = 1.0f;
            _cam2D.OutputSurface = Window;
            _cam2D.LayerMask = BitwiseHelper.Set(_cam2D.LayerMask, 0);
            Engine.Input.Camera = _cam2D;


            //_uiScene = CreateScene("UI");
            //SceneObject camObj = CreateObject(_uiScene);
            //CameraComponent cam = camObj.AddComponent<CameraComponent>();
            //cam.Mode = RenderCameraMode.Orthographic;
            //cam.MaxDrawDistance = 1.0f;
            //_uiScene.AddObject(camObj);
            Window.OnPostResize += UpdateWindownBounds;
            
            UI = _uiLayer.AddObjectWithComponent<UIComponent>();
            //UpdateWindownBounds(Window);


            //_leftPanel = new UIPanel();
            //_leftPanel.ClipPadding.Right = 1;
            //_leftPanel.Margin.SetDock(false, true, false, true);
            //_leftPanel.Width = 300;
            //_leftPanel.Y = 25;
            //UI.AddChild(_leftPanel);

            //// TODO set bounds of UI container to screen size.
            //_menu = new UIMenu();
            //_menu.Height = 25;
            //_menu.Margin.DockLeft = true;
            //_menu.Margin.DockRight = true;
            //_menu.ClipPadding.Bottom = 1;
            //UI.AddChild(_menu);

            //// Test some sub-items
            //UIMenuItem mnuFile = new UIMenuItem();
            //mnuFile.Text = "File";
            //_menu.AddChild(mnuFile);


            // OLD

            //UIMenuItem mnuNew = new UIMenuItem();
            //mnuNew.Label.Text = "New...";
            //mnuNew.BackgroundColor = new Color("#333337");
            //mnuFile.AddChild(mnuNew);

            //UIMenuItem mnuOpen = new UIMenuItem();
            //mnuOpen.Label.Text = "Open";
            //mnuOpen.BackgroundColor = new Color("#333337");
            //mnuFile.AddChild(mnuOpen);

            //UIMenuItem mnuProject = new UIMenuItem();
            //mnuProject.Label.Text = "Project...";
            //mnuProject.BackgroundColor = new Color("#333337");
            //mnuOpen.AddChild(mnuProject);

            //UIMenuItem mnuOpenFile = new UIMenuItem();
            //mnuOpenFile.Label.Text = "File...";
            //mnuOpenFile.BackgroundColor = new Color("#333337");
            //mnuOpen.AddChild(mnuOpenFile);

            //UIMenuItem mnuExit = new UIMenuItem();
            //mnuExit.Label.Text = "Exit";
            //mnuExit.BackgroundColor = new Color("#333337");
            //mnuFile.AddChild(mnuExit);

            //UIMenuItem mnuEdit = new UIMenuItem();
            //mnuEdit.Text = "Edit";
            //_menu.AddChild(mnuEdit);

            _textBox = _uiLayer.AddObjectWithComponent<UITextbox>();
            _textBox.Text = "Test";
            _textBox.IsClippingEnabled = false;
            _textBox.IsVisible = true;
            _textBox.IsEnabled = true;
            _textBox.DepthWriteOverride = GraphicsDepthWritePermission.Disabled;

            _textBox.Object.Transform.LocalPosition = new Vector3F(0, -10, 0);

            UI.AddChild(new UILabel(Engine.Current.DefaultFont, "Label"));

            //var label = _textBox.Object.AddComponent<UILabel>();
            //label.Text = "TestLabel";
            //UI.AddChild(textBox);

        }



        protected override void OnFirstLoad(Engine engine)
        {
            base.OnFirstLoad(engine);
            LoadSystemFontFile("Arial");
        }

        /// <summary>
        /// Hacky method for loading a system font until SpriteFont has a constructor to do the same thing in a nice way.
        /// </summary>
        /// <param name="fontName"></param>
        private void LoadSystemFontFile(string fontName)
        {
            Logger fontLog = Logger.Get();
            fontLog.AddOutput(new LogFileWriter("font{0}.txt"));

            using (FontReader reader = new FontReader(fontName, fontLog))
            {
                FontFile _fontFile = reader.ReadFont(true);
                _textBox.Font = new SpriteFont(Engine.Renderer, _fontFile, 20);
            }
            fontLog.Dispose();
        }


        private void UpdateWindownBounds(ITexture texture)
        {
            INativeSurface window = texture as INativeSurface;
            UI.LocalBounds = new Rectangle(0, 0, window.Width, window.Height);
        }

        protected override void OnUpdate(Timing time)
        {
            //Vector3F position = _textBox.Object.Transform.LocalPosition;

            //position.X++;
            //if (position.X > 100)
            //    position.X = 0;

            //_textBox.Object.Transform.LocalPosition = position;
        }


        /// <summary>
        /// Gets the root UI component which represents the main editor window.
        /// </summary>
        public UIComponent UI { get; private set; }
    }
}
