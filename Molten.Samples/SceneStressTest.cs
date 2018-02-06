﻿using Molten.Graphics;
using Molten.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Samples
{
    public class SceneStressTest : TestGame
    {
        public override string Description => "A simple scene test using spinning, colored cubes";

        Scene _scene;
        List<SceneObject> _objects;
        SceneObject _player;
        Random _rng;
        SpriteText _txtInstructions;
        Vector2 _txtInstructionSize;

        public SceneStressTest(EngineSettings settings = null) : base("Scene Stress", settings) { }

        protected override void OnInitialize(Engine engine)
        {
            base.OnInitialize(engine);

            Window.OnPostResize += Window_OnPostResize;
            
            _rng = new Random();
            _objects = new List<SceneObject>();
            _scene = new Scene("Test", engine);
            SpawnPlayer();

            string text = "[W][A][S][D] to move. Mouse to rotate";
            _txtInstructionSize = TestFont.MeasureString(text);
            _txtInstructions = new SpriteText()
            {
                Text = text,
                Font = TestFont,
                Color = Color.White,
            };

            _scene.AddSprite(_txtInstructions);
            UpdateInstructions();

            string fn = "assets/BasicColor.sbm";
            string source = "";
            using (FileStream stream = new FileStream(fn, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                    source = reader.ReadToEnd();
            }

            ShaderParseResult shaders = engine.Renderer.Resources.CreateShaders(source, fn);
            IMaterial material = shaders["material", 0] as IMaterial;

            IMesh<VertexColor> mesh = Engine.Renderer.Resources.CreateMesh<VertexColor>(36);
            if (material == null)
            {
                Exit();
                return;
            }

            VertexColor[] verts = new VertexColor[]{
                        new VertexColor(new Vector3(-1,-1,-1), Color.Red), //front
                        new VertexColor(new Vector3(-1,1,-1), Color.Red),
                        new VertexColor(new Vector3(1,1,-1), Color.Red),
                        new VertexColor(new Vector3(-1,-1,-1), Color.Red),
                        new VertexColor(new Vector3(1,1,-1), Color.Red),
                        new VertexColor(new Vector3(1,-1,-1), Color.Red),

                        new VertexColor(new Vector3(-1,-1,1), Color.Blue), //back
                        new VertexColor(new Vector3(1,1,1), Color.Blue),
                        new VertexColor(new Vector3(-1,1,1), Color.Blue),
                        new VertexColor(new Vector3(-1,-1,1),Color.Blue),
                        new VertexColor(new Vector3(1,-1,1), Color.Blue),
                        new VertexColor(new Vector3(1,1,1), Color.Blue),

                        new VertexColor(new Vector3(-1,1,-1), Color.Yellow), //top
                        new VertexColor(new Vector3(-1,1,1), Color.Yellow),
                        new VertexColor(new Vector3(1,1,1), Color.Yellow),
                        new VertexColor(new Vector3(-1,1,-1), Color.Yellow),
                        new VertexColor(new Vector3(1,1,1), Color.Yellow),
                        new VertexColor(new Vector3(1,1,-1), Color.Yellow),

                        new VertexColor(new Vector3(-1,-1,-1), Color.Purple), //bottom
                        new VertexColor(new Vector3(1,-1,1), Color.Purple),
                        new VertexColor(new Vector3(-1,-1,1), Color.Purple),
                        new VertexColor(new Vector3(-1,-1,-1), Color.Purple),
                        new VertexColor(new Vector3(1,-1,-1), Color.Purple),
                        new VertexColor(new Vector3(1,-1,1), Color.Purple),

                        new VertexColor(new Vector3(-1,-1,-1), Color.Green), //left
                        new VertexColor(new Vector3(-1,-1,1), Color.Green),
                        new VertexColor(new Vector3(-1,1,1), Color.Green),
                        new VertexColor(new Vector3(-1,-1,-1), Color.Green),
                        new VertexColor(new Vector3(-1,1,1), Color.Green),
                        new VertexColor(new Vector3(-1,1,-1), Color.Green),

                        new VertexColor(new Vector3(1,-1,-1), Color.White), //right
                        new VertexColor(new Vector3(1,1,1), Color.White),
                        new VertexColor(new Vector3(1,-1,1), Color.White),
                        new VertexColor(new Vector3(1,-1,-1), Color.White),
                        new VertexColor(new Vector3(1,1,-1), Color.White),
                        new VertexColor(new Vector3(1,1,1), Color.White),
                    };

            mesh.Material = material;
            mesh.SetVertices(verts);
            for (int i = 0; i < 6000; i++)
                SpawnTestCube(material, mesh, 20);

            Window.PresentClearColor = new Color(20, 20, 20, 255);
        }

        private void UpdateInstructions()
        {
            if (_txtInstructions == null)
                return;

            _txtInstructions.Position = new Vector2()
            {
                X = Window.Width / 2 + (-_txtInstructionSize.X / 2),
                Y = 3,
            };
        }

        private void Window_OnPostResize(ITexture texture)
        {
            UpdateInstructions();
        }

        private void SpawnPlayer()
        {
            _player = Engine.CreateObject(new Vector3(0,0,-5));
            SceneCameraComponent cam = _player.AddComponent<SceneCameraComponent>();
            cam.OutputSurface = Window;
            cam.OutputDepthSurface = WindowDepthSurface;
            _scene.AddObject(_player);
            _scene.OutputCamera = cam;
        }

        private void SpawnTestCube(IMaterial material, IMesh mesh, int spawnRadius)
        {
            SceneObject obj = Engine.CreateObject();
            MeshComponent meshCom = obj.AddComponent<MeshComponent>();
            meshCom.Mesh = mesh;

            int maxRange = spawnRadius * 2;
            obj.Transform.LocalPosition = new Vector3()
            {
                X = -spawnRadius + (float)(_rng.NextDouble() * maxRange),
                Y = -spawnRadius + (float)(_rng.NextDouble() * maxRange),
                Z = spawnRadius + (float)(_rng.NextDouble() * maxRange)
            };

            _objects.Add(obj);
            _scene.AddObject(obj);
        }

        private void Window_OnClose(IWindowSurface surface)
        {
            Exit();
        }

        protected override void OnUpdate(Timing time)
        {
            var rotateAngle = 1.2f * time.Delta;

            foreach(SceneObject obj in _objects)
            {
                obj.Transform.LocalRotationX += rotateAngle;
                obj.Transform.LocalRotationY += rotateAngle;
                obj.Transform.LocalRotationZ += rotateAngle * 0.7f * time.Delta;
            }

            // Mouse input - Messy for now - We're just testing input
            _player.Transform.LocalRotationX += Mouse.Moved.Y;
            _player.Transform.LocalRotationY += Mouse.Moved.X;
            Mouse.CenterInWindow();

            // Keyboard input - Again messy code for now
            Vector3 moveDelta = Vector3.Zero;
            float rotSpeed = 0.25f;
            float speed = 1.0f;

            if (Keyboard.IsPressed(Key.W)) moveDelta += _player.Transform.Global.Backward * rotSpeed;
            if (Keyboard.IsPressed(Key.S)) moveDelta += _player.Transform.Global.Forward * rotSpeed;
            if (Keyboard.IsPressed(Key.A)) moveDelta += _player.Transform.Global.Left * rotSpeed;
            if (Keyboard.IsPressed(Key.D)) moveDelta += _player.Transform.Global.Right * rotSpeed;

            _player.Transform.LocalPosition += moveDelta * time.Delta * speed;
        }
    }
}
