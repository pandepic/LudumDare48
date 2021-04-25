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

            player.TryAddComponent(new TransformComponent()
            {
                Position = position,
                Rotation = 0f,
            });

            player.TryAddComponent(new PhysicsComponent()
            {
                MaxSpeed = new Vector2(400, 400),
                MoveSpeed = 400f,
                JumpSpeed = 550f,
                Acceleration = Vector2.Zero,
                Velocity = Vector2.Zero,
                IsFalling = true,
            });

            var scale = 10;

            player.TryAddComponent(new DrawableMaskComponent()
            {
                Texture = AssetManager.LoadTexture2D("adventurer-bg-08.png"),
                AtlasRect = new Rectangle(0, 0, 50, 37),
                Mask = AssetManager.LoadTexture2D("adventurer.png"),
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

            player.TryAddComponent(new PlayerTag());

            CreateOverlay(player, "adventurer-bg-04.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-07.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-06.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-05.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-03.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-01.png", 0.2f);
            CreateOverlay(player, "adventurer-bg-02.png", 0.2f);

            return player;

        } // CreatePlayer

        public static Entity CreateOverlay(Entity parent, string name, float opacity) {
            var overlay = Registry.CreateEntity();

            overlay.TryAddComponent(new OverlayComponent()
            {
                Texture = AssetManager.LoadTexture2D(name),
                Opacity = opacity,
                Parent = parent,
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
                Texture = AssetManager.LoadTexture2D("platform-bg-01.png"),
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

            return platform;

        } // CreatePlatform
    }
}
