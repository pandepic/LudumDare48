using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public enum MovementType
    {
        Left,
        Right,
        Jump,
    }
    public enum ObjectType
    {
        Background,
        Character,
    }

    public struct TransformComponent
    {
        public Entity Parent;
        public float Rotation;
        public Vector2 Position;

        public Vector2 TransformedPosition
        {
            get
            {
                if (!Parent.IsAlive)
                    return Position;
                else
                {
                    ref var parentTransform = ref Parent.GetComponent<TransformComponent>();
                    var transformMatrix =
                        Matrix3x2.CreateRotation(parentTransform.Rotation) *
                        Matrix3x2.CreateTranslation(parentTransform.TransformedPosition);

                    return Vector2.Transform(Position, transformMatrix);
                }
            }
        }
    } // TransformComponent

    public struct DrawableComponent
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public Texture2D Texture;
        public int Layer;
        public SpriteFlipType FlipType;
    }
    
    public struct DrawableMaskComponent
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public Texture2D Texture;
        public Texture2D Mask;
        public int Layer;
        public SpriteFlipType FlipType;
        public RgbaFloat Color;
    }
    
    public struct OverlayComponent
    {
        public Entity Parent;
        public float Opacity;
        public float Factor;
        public float Scale;
        public Texture2D Texture;
    }

    public struct StartMovementComponent
    {
        public MovementType MovementType;
    }

    public struct StopMovementComponent
    {
        public MovementType MovementType;
    }
    
    public struct MovingPlatformComponent
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public Vector2 Destination;
        public float BaseCooldown;
        public float Cooldown;
        public float MoveSpeed;
        public Entity EntityOnPlatform;
    }
}
