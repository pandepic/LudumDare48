using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static class EntityBuilder
    {
        public static Registry Registry;
        
        public static Entity CreatePlayer(Vector2 startPosition)
        {
            var player = Registry.CreateEntity();
            
            player.TryAddComponent(new TransformComponent()
            {
                Position = startPosition,
                Rotation = 0f,
            });
            
            player.TryAddComponent(new PhysicsComponent()
            {
                MaxSpeed = new Vector2(200, 400),
                MoveSpeed = 200f,
                JumpSpeed = 550f,
                Acceleration = Vector2.Zero,
                Velocity = Vector2.Zero,
                IsFalling = false,
            });
            
            player.TryAddComponent(new DrawableComponent()
            {
                Texture = new Texture2D(50, 50, Veldrid.RgbaByte.Red),
                AtlasRect = new Rectangle(0, 0, 50, 50),
                Origin = new Vector2(50, 50) / 2,
                Scale = new Vector2(1),
                Layer = 2,
                FlipType = SpriteFlipType.None,
            });
            
            player.TryAddComponent(new ColliderComponent()
            {
                EventType = ColliderEventType.None,
                CollisionRect = new Rectangle(0, 0, 50, 50),
            });
            
            return player;
        }
    }
}
