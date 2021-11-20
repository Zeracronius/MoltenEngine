﻿namespace Molten
{
    internal class SceneLayerRemove : SceneChange<SceneLayerRemove>
    {
        public Scene ParentScene;

        public SceneLayer Layer;

        public override void Clear()
        {
            Layer = null;
            ParentScene = null;
        }

        internal override void Process(Scene scene)
        {
            ParentScene.Layers.Remove(Layer);
            Recycle(this);
        }
    }
}
