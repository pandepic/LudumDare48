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
            
            player.TryAddComponent(new DrawableComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.Red),
                AtlasRect = new Rectangle(0, 0, 50, 50),
                Origin = Vector2.Zero,
                Scale = new Vector2(1),
                Layer = 2,
                FlipType = SpriteFlipType.None,
            });
            
            player.TryAddComponent(new ColliderComponent()
            {
                EventType = ColliderEventType.None,
                CollisionRect = new Rectangle(0, 0, 50, 50),
            });
            
            player.TryAddComponent(new PlayerTag());
            
            return player;
            
        } // CreatePlayer
        
        public static Entity CreatePlatform(Vector2 position)
        {
            var platform = Registry.CreateEntity();
            
            platform.TryAddComponent(new TransformComponent()
            {
                Position = position,
                Rotation = 0f,
            });
            
            platform.TryAddComponent(new DrawableComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.Blue),
                AtlasRect = new Rectangle(0, 0, 500, 50),
                Origin = Vector2.Zero,
                Scale = new Vector2(1),
                Layer = 2,
                FlipType = SpriteFlipType.None,
            });
            
            platform.TryAddComponent(new ColliderComponent()
            {
                EventType = ColliderEventType.None,
                CollisionRect = new Rectangle(0, 0, 500, 50),
            });
            
            return platform;
            
        } // CreatePlatform
    }
}
