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
            public float OverlayFactor;
            public float OverlayScale;
        }
        private struct DrawBackgroundItem
        {
            public float Opacity;
            public Rectangle SourceRect;
            public Texture2D Texture;
            public Texture2D Mask;
            public int Layer;
            public RgbaFloat Color;
            public SpriteFlipType FlipType;
            public float OverlayFactor;
            public float OverlayScale;
        }

        private static List<DrawMaskItem> _drawMaskItem = new List<DrawMaskItem>();
        private static List<DrawOverlayItem> _drawOverlayItem = new List<DrawOverlayItem>();
        private static List<DrawBackgroundItem> _drawBackgroundItem = new List<DrawBackgroundItem>();
        private static Dictionary<string, (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>)> _maskBatches = new Dictionary<string, (SpriteBatch2D, SimpleUniformBuffer<Matrix4x4>)>();

        public static void RenderBackground(Group group, Camera2D Camera)
        {
            foreach (var entity in group.Entities)
            {
                ref var overlay = ref entity.GetComponent<BackgroundComponent>();

                _drawBackgroundItem.Add(new DrawBackgroundItem()
                {
                    Opacity = overlay.Opacity,
                    SourceRect = overlay.AtlasRect,
                    Texture = overlay.Texture,
                    Mask = overlay.Mask,
                    Layer = overlay.Layer,
                    OverlayFactor = overlay.Factor,
                    OverlayScale = overlay.Scale,
                });
            }

            if (_drawBackgroundItem.Count == 0)
                return;

            // sort by layer then Y position
            _drawBackgroundItem.Sort((x, y) =>
            {
                var val = x.Layer.CompareTo(y.Layer);

                return val;
            });

            string lastMask = null;
            SpriteBatch2D currentBatch = null;
            SimpleUniformBuffer<Matrix4x4> currentBuffer = null;

            bool beginCalled = false;

            var width = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Width;
            var height = ElementGlobals.GraphicsDevice.SwapchainFramebuffer.Height;

            foreach (var item in _drawBackgroundItem) {
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

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Linear);

                    currentBuffer = new SimpleUniformBuffer<Matrix4x4>(ElementGlobals.GraphicsDevice, "MyUniforms", 1, Veldrid.ShaderStages.Fragment);
                    currentBuffer.SetValue(0, Camera.GetViewMatrix());
                    currentBuffer.UpdateBuffer();

                    var pipeline = SpriteBatch2D.GetDefaultSimplePipeline(ElementGlobals.GraphicsDevice, shader: shader);
                    pipeline.AddPipelineTexture(pipelineTexture);
                    pipeline.AddUniformBuffer(currentBuffer);
                    currentBatch = new SpriteBatch2D((int)width, (int)height, ElementGlobals.GraphicsDevice.SwapchainFramebuffer.OutputDescription, pipeline);

                    _maskBatches.Add(item.Texture.TextureName, (currentBatch, currentBuffer));
                }

                if (!beginCalled) {
                    float tWidth = item.Texture.Width;
                    float tHeight = item.Texture.Height;
                    float ratio = tWidth / tHeight;
                    beginCalled = true;
                    currentBuffer.SetValue(0, Matrix4x4.CreateScale(item.OverlayScale * ratio, item.OverlayScale, 1f) * Matrix4x4.CreateTranslation(Camera.Position.X * item.OverlayFactor, Camera.Position.Y * item.OverlayFactor, 0f));
                    currentBuffer.UpdateBuffer();
                    currentBatch.Begin(SamplerType.Point);
                }
                currentBatch.DrawTexture2D(item.Mask, new Rectangle(0, 0, width, height), sourceRect: item.SourceRect, scale: Vector2.One, origin: Vector2.Zero, rotation: 0f, color: new RgbaFloat(1f, 1f, 1f, item.Opacity), flip: item.FlipType);
            }
            if (beginCalled) {
                beginCalled = false;
                currentBatch.End();
            }

            _drawBackgroundItem.Clear();
        }

        public static void RenderOverlay(Group group, Camera2D Camera)
        {
            var cameraView = Camera.ScaledView;
            
            foreach (var entity in group.Entities)
            {
                ref var overlay = ref entity.GetComponent<OverlayComponent>();
                
                var entityRect = EntityUtility.GetEntityDrawRect(overlay.Parent);
                if (!entityRect.Intersects(cameraView))
                    continue;
                
                ref var transform = ref overlay.Parent.GetComponent<TransformComponent>();
                ref var drawable = ref overlay.Parent.GetComponent<DrawableMaskComponent>();

                _drawOverlayItem.Add(new DrawOverlayItem()
                {
                    Position = transform.TransformedPosition.ToVector2I(),
                    Origin = drawable.Origin,
                    Scale = drawable.Scale,
                    Rotation = transform.Rotation,
                    Opacity = overlay.Opacity,
                    SourceRect = drawable.AtlasRect,
                    Texture = overlay.Texture,
                    Mask = drawable.Mask,
                    Layer = overlay.Layer,
                    Color = drawable.Color,
                    FlipType = drawable.FlipType,
                    OverlayFactor = overlay.Factor,
                    OverlayScale = overlay.Scale,
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

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Linear);

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
                    float width = item.Texture.Width;
                    float height = item.Texture.Height;
                    float ratio = width / height;
                    beginCalled = true;
                    currentBuffer.SetValue(0, Matrix4x4.CreateScale(item.OverlayScale * ratio, item.OverlayScale, 1f) * Matrix4x4.CreateTranslation(Camera.Position.X * item.OverlayFactor, Camera.Position.Y * item.OverlayFactor, 0f));
                    currentBuffer.UpdateBuffer();
                    currentBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
                }

                currentBatch.DrawTexture2D(item.Mask, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: new RgbaFloat(item.Color.R, item.Color.G, item.Color.B, item.Opacity), flip: item.FlipType);
            }
            if (beginCalled) {
                beginCalled = false;
                currentBatch.End();
            }

            _drawOverlayItem.Clear();
        }

        public static void RenderMask(Group group, Camera2D Camera)
        {
            var cameraView = Camera.ScaledView;
            
            foreach (var entity in group.Entities)
            {
                var entityRect = EntityUtility.GetEntityDrawRect(entity);
                if (!entityRect.Intersects(cameraView))
                    continue;
                
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
                    Color = drawable.Color,
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

                    var pipelineTexture = new SimplePipelineTexture2D("fBg", item.Texture, SamplerType.Linear);

                    currentBuffer = new SimpleUniformBuffer<Matrix4x4>(ElementGlobals.GraphicsDevice, "MyUniforms", 1, Veldrid.ShaderStages.Fragment);
                    currentBuffer.SetValue(0, Matrix4x4.Identity);
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
                    currentBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
                }

                currentBatch.DrawTexture2D(item.Mask, item.Position, sourceRect: item.SourceRect, scale: item.Scale, origin: item.Origin, rotation: item.Rotation, color: item.Color, flip: item.FlipType);
            }
            if (beginCalled) {
                beginCalled = false;
                currentBatch.End();
            }

            _drawMaskItem.Clear();

        } // Render
    }
}
