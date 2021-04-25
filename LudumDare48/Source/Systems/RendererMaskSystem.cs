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
        private struct DrawOverlayItem
        {
            public Vector2 Position;
            public Vector2 Origin;
            public Vector2 Scale;
            public float Rotation;
            public float Opacity;
            public Rectangle SourceRect;
            public Texture2D Texture;
            public Texture2D Mask;
            public int Layer;
            public RgbaFloat Color;
            public SpriteFlipType FlipType;
        }

        private static List<DrawMaskItem> _drawMaskItem = new List<DrawMaskItem>();
        private static List<DrawOverlayItem> _drawOverlayItem = new List<DrawOverlayItem>();
        private static Dictionary<string, (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>)> _maskBatches = new Dictionary<string, (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>)>();

        public static void RenderOverlay(Group group, Camera2D Camera)
        {
            foreach (var entity in group.Entities)
            {
                ref var opacity = ref entity.GetComponent<OverlayComponent>();
                ref var transform = ref opacity.Parent.GetComponent<TransformComponent>();
                ref var drawable = ref opacity.Parent.GetComponent<DrawableMaskComponent>();

                _drawOverlayItem.Add(new DrawOverlayItem()
                {
                    Position = transform.TransformedPosition.ToVector2I(),
                    Origin = drawable.Origin,
                    Scale = drawable.Scale,
                    Rotation = transform.Rotation,
                    Opacity = opacity.Opacity,
                    SourceRect = drawable.AtlasRect,
                    Texture = opacity.Texture,
                    Mask = drawable.Mask,
                    Layer = drawable.Layer,
                    Color = RgbaFloat.White,
                    FlipType = drawable.FlipType,
                });
            }

            if (_drawOverlayItem.Count == 0)
                return;

            // sort by layer then Y position
            _drawOverlayItem.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                if (val == 0)
                    val = x.Position.Y.CompareTo(y.Position.Y);

                return val;
            });

            string lastMask = null;
            SpriteBatch2D currentBatch = null;
            SimpleUniformBuffer<Matrix4x4> currentBuffer = null;

            bool beginCalled = false;

            foreach (var item in _drawOverlayItem) {
                if (lastMask != item.Texture.TextureName && beginCalled) {
                    beginCalled = false;
                    currentBatch.End();
                }
                lastMask = item.Texture.TextureName;
                if (_maskBatches.TryGetValue(item.Texture.TextureName, out (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>) batch)) {
                    currentBatch = batch.Item1;
                    currentBuffer = batch.Item2;
                } else {
                    var shader = new SimpleShader(ElementGlobals.GraphicsDevice,
                    File.ReadAllText(AssetManager.GetAssetPath("mask.vert")),
                    File.ReadAllText(AssetManager.GetAssetPath("mask.frag")),
                    ElementEngine.Vertex2DPositionTexCoordsColor.VertexLayout);

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Point);

                    currentBuffer = new SimpleUniformBuffer<Matrix4x4>(ElementGlobals.GraphicsDevice, "MyUniforms", 1, Veldrid.ShaderStages.Fragment);
                    currentBuffer.SetValue(0, Camera.GetViewMatrix());
                    currentBuffer.UpdateBuffer();

                    var pipeline = SpriteBatch2D.GetDefaultSimplePipeline(ElementGlobals.GraphicsDevice, shader: shader);
                    pipeline.AddPipelineTexture(pipelineTexture);
                    pipeline.AddUniformBuffer(currentBuffer);
                    var width = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Width;
                    var height = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Height;
                    currentBatch = new SpriteBatch2D((int)width, (int)height, ElementGlobals.GraphicsDevice.SwapchainFramebuffer.OutputDescription, pipeline);

                    _maskBatches.Add(item.Texture.TextureName, (currentBatch, currentBuffer));
                }

                if (!beginCalled) {
                    beginCalled = true;
                    currentBuffer.SetValue(0, Matrix4x4.CreateScale(2f, 2f, 1f) * Matrix4x4.CreateTranslation(Camera.Position.X * 0.001f, Camera.Position.Y * 0.001f, 0f));
                    currentBuffer.UpdateBuffer();
                    currentBatch.Begin(SamplerType.Linear, Camera.GetViewMatrix());
                }

                currentBatch.DrawTexture2D(item.Mask, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: new RgbaFloat(1f, 1f, 1f, item.Opacity));
            }
            if (beginCalled) {
                beginCalled = false;
                currentBatch.End();
            }

            _drawOverlayItem.Clear();
        }

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
            SimpleUniformBuffer<Matrix4x4> currentBuffer = null;

            bool beginCalled = false;

            foreach (var item in _drawMaskItem) {
                if (lastMask != item.Texture.TextureName && beginCalled) {
                    beginCalled = false;
                    currentBatch.End();
                }
                lastMask = item.Texture.TextureName;
                if (_maskBatches.TryGetValue(item.Texture.TextureName, out (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>) batch)) {
                    currentBatch = batch.Item1;
                    currentBuffer = batch.Item2;
                } else {
                    var shader = new SimpleShader(ElementGlobals.GraphicsDevice,
                    File.ReadAllText(AssetManager.GetAssetPath("mask.vert")),
                    File.ReadAllText(AssetManager.GetAssetPath("mask.frag")),
                    ElementEngine.Vertex2DPositionTexCoordsColor.VertexLayout);

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Point);

                    currentBuffer = new SimpleUniformBuffer<Matrix4x4>(ElementGlobals.GraphicsDevice, "MyUniforms", 1, Veldrid.ShaderStages.Fragment);
                    currentBuffer.SetValue(0, Camera.GetViewMatrix());
                    currentBuffer.UpdateBuffer();

                    var pipeline = SpriteBatch2D.GetDefaultSimplePipeline(ElementGlobals.GraphicsDevice, shader: shader);
                    pipeline.AddPipelineTexture(pipelineTexture);
                    pipeline.AddUniformBuffer(currentBuffer);
                    var width = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Width;
                    var height = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Height;
                    currentBatch = new SpriteBatch2D((int)width, (int)height, ElementGlobals.GraphicsDevice.SwapchainFramebuffer.OutputDescription, pipeline);

                    _maskBatches.Add(item.Texture.TextureName, (currentBatch, currentBuffer));
                }

                if (!beginCalled) {
                    beginCalled = true;
                    currentBuffer.SetValue(0, Matrix4x4.CreateScale(2f, 2f, 1f) * Matrix4x4.CreateTranslation(Camera.Position.X * 0.001f, Camera.Position.Y * 0.001f, 0f));
                    currentBuffer.UpdateBuffer();
                    currentBatch.Begin(SamplerType.Linear, Camera.GetViewMatrix());
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
