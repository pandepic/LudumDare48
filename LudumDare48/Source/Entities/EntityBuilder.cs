using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public enum PlatformType
    {
        Normal,
        Death,
        Moving,
    }

    public static class EntityBuilder
    {
        public static Registry Registry;
        public static int SortLayer = 0;

        public static Entity CreatePlayer(Vector2 position)
        {
            var player = Registry.CreateEntity();
            var spritesheet = AssetManager.LoadTexture2D("adventurer.png");

            player.TryAddComponent(new TransformComponent()
            {
                Position = position,
                Rotation = 0f,
            });

            player.TryAddComponent(new PhysicsComponent()
            {
                MaxSpeed = new Vector2(400, 1000),
                MoveSpeed = 850f,
                JumpSpeed = 900f,
                Acceleration = Vector2.Zero,
                Velocity = Vector2.Zero,
                IsFalling = true,
            });

            var scale = 8;

            player.TryAddComponent(new DrawableMaskComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.Black),
                AtlasRect = new Rectangle(0, 0, 50, 37),
                Mask = spritesheet,
                Origin = Vector2.Zero,
                Scale = new Vector2(scale),
                Layer = 2,
                FlipType = SpriteFlipType.None,
                Color = RgbaFloat.White,
            });

            player.TryAddComponent(new ColliderComponent()
            {
                EventType = ColliderEventType.None,
                CollisionRect = new Rectangle(20, 7, 11, 29),
                Scale = new Vector2(scale),
            });

            player.TryAddComponent(new PlayerComponent()
            {
                RespawnPosition = position,
            });

            player.TryAddComponent(new SpriteComponent()
            {
                FrameSize = new Vector2I(50, 37),
                Size = spritesheet.Size,
            });

            player.TryAddComponent(new PlayerAnimationComponent());

            CreateOverlay(player, "adventurer-bg-08.jpg");
            CreateOverlay(player, "adventurer-bg-04.jpg");
            CreateOverlay(player, "adventurer-bg-07.jpg");
            CreateOverlay(player, "adventurer-bg-06.jpg");
            CreateOverlay(player, "adventurer-bg-05.jpg");
            CreateOverlay(player, "adventurer-bg-03.jpg");
            CreateOverlay(player, "adventurer-bg-01.jpg");
            CreateOverlay(player, "adventurer-bg-02.jpg");

            EntityUtility.PlaySpriteAnimation(player, AnimationType.Idle);

            return player;

        } // CreatePlayer

        public static Entity CreateOverlay(Entity parent, string name, float opacity = 0.2f, float factor = 0.001f, float scale = 2f) {
            var overlay = Registry.CreateEntity();

            overlay.TryAddComponent(new OverlayComponent()
            {
                Texture = AssetManager.LoadTexture2D(name),
                Opacity = opacity,
                Parent = parent,
                Factor = factor,
                Scale = scale,
                Layer = SortLayer++,
            });

            return overlay;
        }

        public static Entity CreateBackground(string name, float opacity = 0.1f, float factor = 0.0001f, float scale = 0.1f) {
            var overlay = Registry.CreateEntity();

            overlay.TryAddComponent(new BackgroundComponent()
            {
                Texture = AssetManager.LoadTexture2D(name),
                AtlasRect = new Rectangle(0, 0, 50, 50),
                Mask = new Texture2D(50, 50, Veldrid.RgbaByte.White),
                Opacity = opacity,
                Factor = factor,
                Scale = scale,
                Layer = SortLayer++,
            });

            return overlay;
        }

        public static Entity CreatePlatform(Vector2 position, PlatformType type)
        {
            var collisionType = ColliderEventType.None;
            var color = RgbaFloat.White;

            switch (type)
            {
                case PlatformType.Death:
                {
                    collisionType = ColliderEventType.Death;
                    color = RgbaFloat.Red;
                }
                break;
            }

            var platform = Registry.CreateEntity();

            platform.TryAddComponent(new TransformComponent()
            {
                Position = position,
                Rotation = 0f,
            });

            var scale = 4f;

            platform.TryAddComponent(new DrawableMaskComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.White),
                AtlasRect = new Rectangle(22, 34, 89, 38),
                Mask = AssetManager.LoadTexture2D("platforms-mask.png"),
                Origin = Vector2.Zero,
                Scale = new Vector2(scale),
                Layer = 2,
                FlipType = SpriteFlipType.None,
                Color = color,
            });

            platform.TryAddComponent(new ColliderComponent()
            {
                EventType = collisionType,
                CollisionRect = new Rectangle(4, 9, 81, 25),
                Scale = new Vector2(scale),
            });

            CreateOverlay(platform, "platform-bg-01.jpg");
            CreateOverlay(platform, "platform-bg-02.jpg");
            CreateOverlay(platform, "platform-bg-03.jpg");
            CreateOverlay(platform, "platform-bg-04.jpg");
            CreateOverlay(platform, "platform-bg-05.jpg");
            CreateOverlay(platform, "platform-bg-06.jpg");
            CreateOverlay(platform, "platform-bg-07.jpg");
            CreateOverlay(platform, "platform-bg-08.jpg");
            CreateOverlay(platform, "platform-bg-09.jpg");
            CreateOverlay(platform, "platform-bg-10.jpg");
            CreateOverlay(platform, "platform-bg-11.jpg");
            CreateOverlay(platform, "platform-bg-12.jpg");

            return platform;

        } // CreatePlatform
        
        public static Entity CreateRecording(string asset, Vector2 position, Vector2 respawnPosition, bool isLastRecording)
        {
            var recording = Registry.CreateEntity();
            
            recording.TryAddComponent(new RecordingComponent()
            {
                HasPlayed = false,
                Asset = asset,
                Position = position,
                IsLastRecording = isLastRecording,
                RespawnPosition = respawnPosition,
            });
            
            return recording;
        }
        
        public static void CreateBackgrounds() {
            CreateBackground("bg-01.jpg");
            CreateBackground("bg-02.jpg");
            CreateBackground("bg-03.jpg");
            CreateBackground("bg-04.jpg");
            CreateBackground("bg-05.jpg");
            CreateBackground("bg-06.jpg");
            CreateBackground("bg-07.jpg");
            CreateBackground("bg-08.jpg");
            CreateBackground("bg-09.jpg");
        }
    }
}
