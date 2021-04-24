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
            });
            
            player.TryAddComponent(new PhysicsComponent()
            {
                MaxSpeed = new Vector2(200, 400),
            });
            
            player.TryAddComponent(new DrawableComponent());
            player.TryAddComponent(new ColliderComponent());
            
            return player;
        }
    }
}
