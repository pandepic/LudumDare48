using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static class EntityBuilder
    {
        public static Registry Registry;

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
                MoveSpeed = 800f,
                JumpSpeed = 550f,
                Acceleration = Vector2.Zero,
                Velocity = Vector2.Zero,
                IsFalling = true,
            });

            var scale = 10;

            player.TryAddComponent(new DrawableMaskComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.White),
                AtlasRect = new Rectangle(0, 0, 50, 37),
                Mask = spritesheet,
                Origin = Vector2.Zero,
                Scale = new Vector2(scale),
                Layer = 2,
                FlipType = SpriteFlipType.None,
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
            
            CreateOverlay(player, "adventurer-bg-08.png");
            CreateOverlay(player, "adventurer-bg-04.png");
            CreateOverlay(player, "adventurer-bg-07.png");
            CreateOverlay(player, "adventurer-bg-06.png");
            CreateOverlay(player, "adventurer-bg-05.png");
            CreateOverlay(player, "adventurer-bg-03.png");
            CreateOverlay(player, "adventurer-bg-01.png");
            CreateOverlay(player, "adventurer-bg-02.png");
            
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
            });

            return overlay;
        }

        public static Entity CreatePlatform(Vector2 position)
        {
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
            });

            platform.TryAddComponent(new ColliderComponent()
            {
                EventType = ColliderEventType.None,
                CollisionRect = new Rectangle(4, 9, 81, 25),
                Scale = new Vector2(scale),
            });

            CreateOverlay(platform, "platform-bg-01.png");
            CreateOverlay(platform, "platform-bg-02.png");
            CreateOverlay(platform, "platform-bg-03.png");
            CreateOverlay(platform, "platform-bg-04.png");
            CreateOverlay(platform, "platform-bg-05.png");
            CreateOverlay(platform, "platform-bg-06.png");
            CreateOverlay(platform, "platform-bg-07.png");
            CreateOverlay(platform, "platform-bg-08.png");
            CreateOverlay(platform, "platform-bg-09.png");
            CreateOverlay(platform, "platform-bg-10.png");
            CreateOverlay(platform, "platform-bg-11.png");
            CreateOverlay(platform, "platform-bg-12.png");

            return platform;

        } // CreatePlatform
    }
}
