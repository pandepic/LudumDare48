using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        public static void SpriteAnimation(Group group, GameTimer gameTimer)
        {
            foreach (var entity in group.Entities)
            {
                ref var animation = ref entity.GetComponent<SpriteAnimationComponent>();
                
                if (animation.CurrentFrame >= animation.EndFrame && animation.CurrentFrameTime <= 0 && !animation.Loop)
                    continue;
                
                animation.CurrentFrameTime -= gameTimer.DeltaS;
                
                if (animation.CurrentFrameTime <= 0)
                {
                    if (animation.CurrentFrame >= animation.EndFrame)
                    {
                        if (animation.Loop)
                        {
                            animation.CurrentFrame = animation.StartFrame;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        animation.CurrentFrame += 1;
                    }
                    
                    animation.CurrentFrameTime = animation.BaseFrameTime;
                    EntityUtility.SetEntitySpriteFrame(entity, animation.CurrentFrame);
                }
            }
        } // SpriteAnimation
        
        public static void PlayerAnimation(Group group)
        {
            foreach (var entity in group.Entities)
            {
                ref var physics = ref entity.GetComponent<PhysicsComponent>();
                ref var animation = ref entity.GetComponent<SpriteAnimationComponent>();
                ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();
                
                if (physics.Velocity.X < 0)
                    drawable.FlipType = SpriteFlipType.Horizontal;
                else if (physics.Velocity.X > 0)
                    drawable.FlipType = SpriteFlipType.None;
                
                var hasAnimation = entity.HasComponent<SpriteAnimationComponent>();
                
                if (physics.Velocity.X == 0)
                {
                    if (hasAnimation && animation.Type == AnimationType.Idle)
                        return;
                    
                    EntityUtility.PlaySpriteAnimation(entity, AnimationType.Idle);
                }
                else
                {
                    if (hasAnimation && animation.Type == AnimationType.Running)
                        return;
                    
                    EntityUtility.PlaySpriteAnimation(entity, AnimationType.Running);
                }
            }
        }
        
    } // Systems
}