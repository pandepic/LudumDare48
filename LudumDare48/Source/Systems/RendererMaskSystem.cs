using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        private struct DrawMaskItem
        {
            public Vector2 Position;
            public Vector2 Origin;
            public Vector2 Scale;
            public float Rotation;
            public Rectangle SourceRect;
            public Texture2D Texture;
            public Texture2D Mask;
            public int Layer;
            public RgbaFloat Color;
            public SpriteFlipType FlipType;
        }

        private static List<DrawMaskItem> _drawMaskItem = new List<DrawMaskItem>();
        private static Dictionary<string, SpriteBatch2D> _maskBatches = new Dictionary<string, SpriteBatch2D>();

        public static void RenderMask(Group group, Camera2D Camera)
        {
            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<TransformComponent>();
                ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();

                _drawMaskItem.Add(new DrawMaskItem()
                {
                    Position = transform.TransformedPosition.ToVector2I(),
                    Origin = drawable.Origin,
                    Scale = drawable.Scale,
                    Rotation = transform.Rotation,
                    SourceRect = drawable.AtlasRect,
                    Texture = drawable.Texture,
                    Mask = drawable.Mask,
                    Layer = drawable.Layer,
                    Color = RgbaFloat.White,
                    FlipType = drawable.FlipType,
                });
            }

            if (_drawMaskItem.Count == 0)
                return;

            // sort by layer then Y position
            _drawMaskItem.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                if (val == 0)
                    val = x.Position.Y.CompareTo(y.Position.Y);

                return val;
            });

            string lastMask = null;
            SpriteBatch2D currentBatch = null;
            bool beginCalled = false;

            foreach (var item in _drawMaskItem) {
                if (lastMask != item.Texture.TextureName && beginCalled) {
                    beginCalled = false;
                    currentBatch.End();
                }
                lastMask = item.Texture.TextureName;
                if (_maskBatches.TryGetValue(item.Texture.TextureName, out SpriteBatch2D batch)) {
                    currentBatch = batch;
                } else {
                    var shader = new SimpleShader(ElementGlobals.GraphicsDevice,
                    File.ReadAllText(AssetManager.GetAssetPath("testmask.vert")),
                    File.ReadAllText(AssetManager.GetAssetPath("testmask.frag")),
                    ElementEngine.Vertex2DPositionTexCoordsColor.VertexLayout);

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Point);

                    var uniformBuffer = new SimpleUniformBuffer<Matrix4x4>(ElementGlobals.GraphicsDevice, "MyUniforms", 1, Veldrid.ShaderStages.Fragment);
                    uniformBuffer.SetValue(0, Camera.GetViewMatrix());
                    uniformBuffer.UpdateBuffer();

                    var pipeline = SpriteBatch2D.GetDefaultSimplePipeline(ElementGlobals.GraphicsDevice, shader: shader);
                    pipeline.AddPipelineTexture(pipelineTexture);
                    pipeline.AddUniformBuffer(uniformBuffer);
                    var width = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Width;
                    var height = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Height;
                    currentBatch = new SpriteBatch2D((int)width, (int)height, ElementGlobals.GraphicsDevice.SwapchainFramebuffer.OutputDescription, pipeline);

                    _maskBatches.Add(item.Texture.TextureName, currentBatch);
                }

                if (!beginCalled) {
                    beginCalled = true;
                    currentBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
                }

                currentBatch.DrawTexture2D(item.Mask, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: item.Color);
            }
            if (beginCalled) {
                beginCalled = false;
                currentBatch.End();
            }

            _drawMaskItem.Clear();

        } // Render
    }
}
