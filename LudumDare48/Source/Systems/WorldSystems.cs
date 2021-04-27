using System;
using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        public static void Death(Group group)
        {
            foreach (var entity in group.Entities)
            {
                if (entity.HasComponent<PlayerComponent>())
                {
                    entity.RemoveComponent<DeathTag>();
                    
                    ref var transform = ref entity.GetComponent<TransformComponent>();
                    ref var player = ref entity.GetComponent<PlayerComponent>();
                    ref var physics = ref entity.GetComponent<PhysicsComponent>();
                    
                    transform.Position = player.RespawnPosition;
                    physics.Velocity = Vector2.Zero;
                    physics.Acceleration = Vector2.Zero;
                }
                else
                {
                    group.Registry.DestroyEntity(entity);
                }
            }
        }

        public static void StartMovement(Group group)
        {
            foreach (var entity in group.Entities)
            {
                ref var movement = ref entity.GetComponent<StartMovementComponent>();
                ref var physics = ref entity.GetComponent<PhysicsComponent>();
                ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();

                switch (movement.MovementType)
                {
                    case MovementType.Left:
                        physics.Acceleration.X += -physics.MoveSpeed;
                        physics.Velocity.X = 0;
                        break;

                    case MovementType.Right:
                        physics.Acceleration.X += physics.MoveSpeed;
                        physics.Velocity.X = 0;
                        break;

                    case MovementType.Jump:
                        physics.Velocity.Y = -physics.JumpSpeed;
                        break;
                }

                entity.RemoveComponent<StartMovementComponent>();
            }
        }

        public static void StopMovement(Group group)
        {
            foreach (var entity in group.Entities)
            {
                ref var movement = ref entity.GetComponent<StopMovementComponent>();
                ref var physics = ref entity.GetComponent<PhysicsComponent>();
                ref var drawable = ref entity.GetComponent<DrawableMaskComponent>();

                switch (movement.MovementType)
                {
                    case MovementType.Left:
                        physics.Acceleration.X += physics.MoveSpeed;
                        physics.Velocity.X = 0;
                        drawable.FlipType = SpriteFlipType.Horizontal;
                        break;

                    case MovementType.Right:
                        physics.Acceleration.X += -physics.MoveSpeed;
                        physics.Velocity.X = 0;
                        drawable.FlipType = SpriteFlipType.None;
                        break;
                }
                
                entity.RemoveComponent<StopMovementComponent>();
            }
        } // StopMovement
        
        public static void MovingPlatforms(Group group, GameTimer gameTimer)
        {
            foreach (var entity in group.Entities)
            {
                ref var transform = ref entity.GetComponent<TransformComponent>();
                ref var movingPlatform = ref entity.GetComponent<MovingPlatformComponent>();
                
                if (movingPlatform.Cooldown > 0)
                {
                    movingPlatform.Cooldown -= gameTimer.DeltaS;
                    continue;
                }
                
                if (transform.TransformedPosition.GetDistance(movingPlatform.Destination) < 2f)
                {
                    if (movingPlatform.Destination == movingPlatform.StartPosition)
                        movingPlatform.Destination = movingPlatform.EndPosition;
                    else
                        movingPlatform.Destination = movingPlatform.StartPosition;
                    
                    movingPlatform.Cooldown = movingPlatform.BaseCooldown;
                }
                
                var direction = Vector2.Zero;

                if (transform.TransformedPosition.X > movingPlatform.Destination.X)
                    direction.X = -1;
                else if (transform.TransformedPosition.X < movingPlatform.Destination.X)
                    direction.X = 1;

                if (transform.TransformedPosition.Y > movingPlatform.Destination.Y)
                    direction.Y = -1;
                else if (transform.TransformedPosition.Y < movingPlatform.Destination.Y)
                    direction.Y = 1;

                var velocity = direction * movingPlatform.MoveSpeed;
                transform.Position += velocity * gameTimer.DeltaS;
                
                if (movingPlatform.EntityOnPlatform.IsAlive)
                {
                    ref var entityTransform = ref movingPlatform.EntityOnPlatform.GetComponent<TransformComponent>();
                    entityTransform.Position += velocity * gameTimer.DeltaS;
                }
            }
        } // MovingPlatforms
        
        public static void Recordings(Group group, Entity player, GameStatePlay gameState)
        {
            foreach (var entity in group.Entities)
            {
                ref var recording = ref entity.GetComponent<RecordingComponent>();
                ref var playerTransform = ref player.GetComponent<TransformComponent>();
                ref var playerComponent = ref player.GetComponent<PlayerComponent>();
                
                if (recording.HasPlayed)
                    continue;
                
                if (playerTransform.TransformedPosition.GetDistance(recording.Position) < 400f)
                {
                    SoundManager.Play(recording.Asset, 1);
                    recording.HasPlayed = true;
                    
                    playerComponent.RespawnPosition = recording.RespawnPosition;
                    
                    if (recording.IsLastRecording)
                    {
                        gameState.Win();
                        SoundManager.StopByType(0);
                    }
                }
            }
        } // Recordings
    }
}
