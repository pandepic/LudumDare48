using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        private struct DrawItem
        {
            public Vector2 Position;
            public Vector2 Origin;
            public Vector2 Scale;
            public float Rotation;
            public Rectangle SourceRect;
            public Texture2D Texture;
            public int Layer;
            public RgbaFloat Color;
            public SpriteFlipType FlipType;
        }
        
        private static List<DrawItem> _drawList = new List<DrawItem>();
        
        public static void Render(Group group, SpriteBatch2D spriteBatch)
        {
            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<TransformComponent>();
                ref var drawable = ref entity.GetComponent<DrawableComponent>();
                
                _drawList.Add(new DrawItem()
                {
                    Position = transform.TransformedPosition.ToVector2I(),
                    Origin = drawable.Origin,
                    Scale = drawable.Scale,
                    Rotation = transform.Rotation,
                    SourceRect = drawable.AtlasRect,
                    Texture = drawable.Texture,
                    Layer = drawable.Layer,
                    Color = RgbaFloat.White,
                    FlipType = drawable.FlipType,
                });
            }
            
            if (_drawList.Count == 0)
                return;
            
            // sort by layer then Y position
            _drawList.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                if (val == 0)
                    val = x.Position.Y.CompareTo(y.Position.Y);

                return val;
            });
            
            foreach (var item in _drawList)
                spriteBatch.DrawTexture2D(item.Texture, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: item.Color);
            
            _drawList.Clear();
            
        } // Render
    }
}
