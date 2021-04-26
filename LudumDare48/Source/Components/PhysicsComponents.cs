using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public enum ColliderEventType
    {
        None,
        Death,
        MovingPlatform,
    }
    
    public struct PhysicsComponent
    {
        public float MoveSpeed;
        public float JumpSpeed;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public Vector2 MoveAmount;
        public Vector2 MaxSpeed;
        public bool IsFalling;
        public Entity OnMovingPlatform;
    }
    
    public struct ColliderComponent
    {
        public ColliderEventType EventType;
        public Rectangle CollisionRect;
        public Vector2 Scale;
    }
    
    public struct ColliderEventComponent
    {
        public ColliderEventType EventType;
        public Entity CollidedWith;
    }
}
