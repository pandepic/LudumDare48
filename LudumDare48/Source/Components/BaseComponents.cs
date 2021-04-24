using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public enum MovementType
    {
        Left,
        Right,
        Jump,
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
    
    public struct MovementComponent
    {
        public MovementType MovementType;
    }
}
