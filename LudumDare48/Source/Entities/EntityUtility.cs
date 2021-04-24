using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static class EntityUtility
    {
        public static Rectangle GetEntityDrawRect(Entity entity)
        {
            ref var transform = ref entity.GetComponent<TransformComponent>();
            ref var drawable = ref entity.GetComponent<DrawableComponent>();
            
            return new Rectangle(transform.TransformedPosition, drawable.AtlasRect.SizeF * drawable.Scale);
        }
        
        public static Rectangle GetEntityCollisionRect(Entity entity)
        {
            ref var transform = ref entity.GetComponent<TransformComponent>();
            ref var collider = ref entity.GetComponent<ColliderComponent>();
            
            return new Rectangle(collider.CollisionRect.Location.ToVector2() + transform.TransformedPosition, collider.CollisionRect.SizeF * collider.Scale);
        }
    }
}