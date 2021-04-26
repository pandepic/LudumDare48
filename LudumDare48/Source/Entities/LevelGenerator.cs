using System;
using System.IO;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

namespace LudumDare48
{
    public static class LevelGenerator
    {
        public static void GenerateLevel(GameStatePlay gameState)
        {
            var rng = new Random();
            var platforms = 100;
            var jumpHeight = 100;
            var jumpLength = 200;
            
            var prevPlatformPosition = Vector2.Zero;
            var nextPlatformPosition = new Vector2(25, 500);
            var prevMovingPlatform = false;
            
            for (var i = 0; i < platforms; i++)
            {
                var platform = EntityBuilder.CreatePlatform(nextPlatformPosition, PlatformType.Normal);
                ref var drawable = ref platform.GetComponent<DrawableMaskComponent>();
                
                var drawRect = EntityUtility.GetEntityDrawRect(platform);
                if (drawRect.Bottom > gameState.DeathHeight)
                    gameState.DeathHeight = drawRect.Bottom;
                
                var platformSize = drawable.AtlasRect.SizeF * drawable.Scale;
                var verticalDirection = rng.Next(0, 10) < 5 ? -1 : 1;
                var platformOffset = new Vector2(platformSize.X + jumpLength, verticalDirection * (platformSize.Y + jumpHeight));
                
                if (i > 3 && rng.Next(0, 10) < 3)
                {
                    var secondVerticalDirection = prevPlatformPosition.Y < nextPlatformPosition.Y ? 1 : -1;
                    
                    var secondPlatformPosition = prevPlatformPosition
                        + new Vector2(
                            (platformSize.X + jumpLength) / 2,
                            ((platformSize.Y + jumpHeight) / 2) * secondVerticalDirection);
                    
                    EntityBuilder.CreatePlatform(secondPlatformPosition, PlatformType.Death);
                    prevMovingPlatform = false;
                }
                else if (i > 3 && rng.Next(0, 10) < 3 && !prevMovingPlatform)
                {
                    var endPosition = nextPlatformPosition + new Vector2(platformOffset.X * 2f, platformOffset.Y);
                    
                    platform.TryAddComponent(new MovingPlatformComponent()
                    {
                        StartPosition = nextPlatformPosition,
                        EndPosition = endPosition,
                        Destination = endPosition,
                        MoveSpeed = 100,
                        BaseCooldown = 2f,
                        Cooldown = 2f,
                    });
                    
                    ref var collider = ref platform.GetComponent<ColliderComponent>();
                    collider.EventType = ColliderEventType.MovingPlatform;
                    
                    nextPlatformPosition = endPosition;
                    prevMovingPlatform = true;
                }
                else
                {
                    prevMovingPlatform = false;
                }
                
                prevPlatformPosition = nextPlatformPosition;
                nextPlatformPosition += platformOffset;
            }
            
            gameState.DeathHeight += 500;
            
        } // GenerateLevel
    } // LevelGenerator
}
