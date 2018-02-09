﻿using Molten.Graphics;
using Molten.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Samples
{
    public class SceneIndexedStressTest : TestGame
    {
        public override string Description => "A simple scene test using colored cubes with indexed meshes.";

        Scene _scene;
        List<SceneObject> _objects;
        SceneObject _player;
        Random _rng;
        SpriteText _txtInstructions;
        Vector2 _txtInstructionSize;

        public SceneIndexedStressTest(EngineSettings settings = null) : base("Scene Stress (Indexed)", settings) { }

        protected override void OnInitialize(Engine engine)
        {
            base.OnInitialize(engine);

            Window.OnPostResize += Window_OnPostResize;
            
            _rng = new Random();
            _objects = new List<SceneObject>();
            _scene = CreateScene("Test");
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

            IIndexedMesh<VertexColor> mesh = Engine.Renderer.Resources.CreateIndexedMesh<VertexColor>(24, 36);
            if (material == null)
            {
                Exit();
                return;
            }

            VertexColor[] vertices = new VertexColor[]{
                new VertexColor(new Vector3(-1,-1,-1), Color.Red), //front
                new VertexColor(new Vector3(-1,1,-1), Color.Red),
                new VertexColor(new Vector3(1,1,-1), Color.Red),
                new VertexColor(new Vector3(1,-1,-1), Color.Red),

                new VertexColor(new Vector3(-1,-1,1), Color.Green), //back
                new VertexColor(new Vector3(1,1,1), Color.Green),
                new VertexColor(new Vector3(-1,1,1), Color.Green),
                new VertexColor(new Vector3(1,-1,1), Color.Green),

                new VertexColor(new Vector3(-1,1,-1), Color.Blue), //top
                new VertexColor(new Vector3(-1,1,1), Color.Blue),
                new VertexColor(new Vector3(1,1,1), Color.Blue),
                new VertexColor(new Vector3(1,1,-1), Color.Blue),

                new VertexColor(new Vector3(-1,-1,-1), Color.Yellow), //bottom
                new VertexColor(new Vector3(1,-1,1), Color.Yellow),
                new VertexColor(new Vector3(-1,-1,1), Color.Yellow),
                new VertexColor(new Vector3(1,-1,-1), Color.Yellow),

                new VertexColor(new Vector3(-1,-1,-1), Color.Purple), //left
                new VertexColor(new Vector3(-1,-1,1), Color.Purple),
                new VertexColor(new Vector3(-1,1,1), Color.Purple),
                new VertexColor(new Vector3(-1,1,-1), Color.Purple),

                new VertexColor(new Vector3(1,-1,-1), Color.White), //right
                new VertexColor(new Vector3(1,1,1), Color.White),
                new VertexColor(new Vector3(1,-1,1), Color.White),
                new VertexColor(new Vector3(1,1,-1), Color.White),
            };

            int[] indices = new int[]{
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 7, 5,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 15, 13,
                16,17,18, 16, 18, 19,
                20, 21, 22, 20, 23, 21,
            };

            mesh.Material = material;
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices);
            for (int i = 0; i < 6000; i++)
                SpawnTestCube(material, mesh, 70);

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
            _player = CreateObject(new Vector3(0,0,-5));
            SceneCameraComponent cam = _player.AddComponent<SceneCameraComponent>();
            cam.OutputSurface = Window;
            cam.OutputDepthSurface = WindowDepthSurface;
            _scene.AddObject(_player);
            _scene.OutputCamera = cam;
        }

        private void SpawnTestCube(IMaterial material, IMesh mesh, int spawnRadius)
        {
            SceneObject obj = CreateObject();
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

            // Keyboard input - Again messy code for now
            Vector3 moveDelta = Vector3.Zero;
            float rotSpeed = 0.25f;
            float speed = 1.0f;

            // Mouse input - Messy for now - We're just testing input
            _player.Transform.LocalRotationX -= Mouse.Moved.Y * rotSpeed;
            _player.Transform.LocalRotationY += Mouse.Moved.X * rotSpeed;
            Mouse.CenterInWindow();

            if (Keyboard.IsPressed(Key.W)) moveDelta += _player.Transform.Global.Backward * speed;
            if (Keyboard.IsPressed(Key.S)) moveDelta += _player.Transform.Global.Forward * speed;
            if (Keyboard.IsPressed(Key.A)) moveDelta += _player.Transform.Global.Left * speed;
            if (Keyboard.IsPressed(Key.D)) moveDelta += _player.Transform.Global.Right * speed;

            _player.Transform.LocalPosition += moveDelta * time.Delta * speed;

            base.OnUpdate(time);
        }
    }
}