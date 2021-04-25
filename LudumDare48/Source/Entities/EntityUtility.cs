using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public enum AnimationType
    {
        Idle,
        Running,
        Jumping,
    }
    
    public class AnimationData
    {
        public int StartFrame;
        public int EndFrame;
        public float FrameTime;
        public bool Loop;
    }
    
    public static class EntityUtility
    {
        public static Dictionary<AnimationType, AnimationData> Animations = new Dictionary<AnimationType, AnimationData>()
        {
            {
                AnimationType.Idle,
                new AnimationData()
                {
                    StartFrame = 1,
                    EndFrame = 4,
                    FrameTime = 0.2f,
                    Loop = true,
                }
            },
            
            {
                AnimationType.Running,
                new AnimationData()
                {
                    StartFrame = 10,
                    EndFrame = 14,
                    FrameTime = 0.2f,
                    Loop = true,
                }
            },
        };
        
        public static Rectangle GetEntityDrawRect(Entity entity)
        {
            ref var transform = ref entity.GetComponent<TransformComponent>();
            ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();

            return new Rectangle(transform.TransformedPosition, drawable.AtlasRect.SizeF * drawable.Scale);
        }

        public static Rectangle GetEntityCollisionRect(Entity entity)
        {
            ref var transform = ref entity.GetComponent<TransformComponent>();
            ref var collider = ref entity.GetComponent<ColliderComponent>();

            return new Rectangle(collider.CollisionRect.Location.ToVector2() * collider.Scale + transform.TransformedPosition, collider.CollisionRect.SizeF * collider.Scale);
        }
        
        public static void PlaySpriteAnimation(Entity entity, AnimationType animation)
        {
            if (!Animations.TryGetValue(animation, out var animData))
                return;
            
            entity.TryAddComponent(new SpriteAnimationComponent()
            {
                Type = animation,
                StartFrame = animData.StartFrame,
                EndFrame = animData.EndFrame,
                BaseFrameTime = animData.FrameTime,
                CurrentFrameTime = animData.FrameTime,
                CurrentFrame = animData.StartFrame,
                Loop = animData.Loop,
            });
        }
        
        public static void SetEntitySpriteFrame(Entity entity, int frameIndex)
        {
            ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();
            ref var sprite = ref entity.GetComponent<SpriteComponent>();
            
            drawable.AtlasRect.X = ((frameIndex - 1) % (sprite.Size.X / sprite.FrameSize.X)) * sprite.FrameSize.X;
            drawable.AtlasRect.Y = ((frameIndex - 1) / (sprite.Size.X / sprite.FrameSize.X)) * sprite.FrameSize.Y;
            drawable.AtlasRect.Width = sprite.FrameSize.X;
            drawable.AtlasRect.Height = sprite.FrameSize.Y;
        }
    }
}
