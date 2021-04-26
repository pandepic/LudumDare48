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
        public static void Physics(Group physicsGroup, Group colliderGroup, GameTimer gameTimer, float gravity, int moveStep, float deathHeight)
        {
            foreach (var entity in physicsGroup.Entities)
            {
                ref var transform = ref entity.GetComponent<TransformComponent>();
                ref var physics = ref entity.GetComponent<PhysicsComponent>();

                physics.Velocity += physics.Acceleration * gameTimer.DeltaS;
                physics.Velocity.Y += gravity * gameTimer.DeltaS;

                if (physics.Velocity.X > physics.MaxSpeed.X)
                    physics.Velocity.X = physics.MaxSpeed.X;
                if (physics.Velocity.X < -physics.MaxSpeed.X)
                    physics.Velocity.X = -physics.MaxSpeed.X;
                if (physics.Velocity.Y > physics.MaxSpeed.Y)
                    physics.Velocity.Y = physics.MaxSpeed.Y;
                if (physics.Velocity.Y < -physics.MaxSpeed.Y)
                    physics.Velocity.Y = -physics.MaxSpeed.Y;

                if (!entity.HasComponent<ColliderComponent>())
                    transform.Position += physics.Velocity * gameTimer.DeltaS;
                else
                    CollisionMovement(entity, ref transform, ref physics, colliderGroup, gameTimer, moveStep);
                
                if (physics.IsFalling && physics.OnMovingPlatform.IsAlive)
                {
                    ref var movingPlatform = ref physics.OnMovingPlatform.GetComponent<MovingPlatformComponent>();
                    movingPlatform.EntityOnPlatform = new Entity();
                    physics.OnMovingPlatform = new Entity();
                }
                
                if (transform.TransformedPosition.Y >= deathHeight && entity.HasComponent<PlayerComponent>())
                {
                    entity.TryAddComponent(new DeathTag());
                }
            }

        } // Physics

        private static void CollisionMovement(Entity entity, ref TransformComponent transform, ref PhysicsComponent physics, Group colliderGroup, GameTimer gameTimer, int moveStep)
        {
            ref var collider = ref entity.GetComponent<ColliderComponent>();

            var directionY = -1f;
            if (physics.Velocity.Y > 0)
                directionY = 1f;

            var directionX = -1f;
            if (physics.Velocity.X > 0)
                directionX = 1f;
            
            physics.MoveAmount += physics.Velocity * gameTimer.DeltaS;
            
            var movement = physics.MoveAmount.ToVector2I();
            
            if (movement == Vector2I.Zero)
                return;
            
            if (movement.X < 0)
                movement.X *= -1;
            if (movement.Y < 0)
                movement.Y *= -1;
            
            if (movement.Y > 0)
                physics.IsFalling = true;
            
            while (movement.Y > 0)
            {
                var step = moveStep;
                if (step > movement.Y)
                    step = movement.Y;

                movement.Y -= step;
                physics.MoveAmount.Y -= step * directionY;
                transform.Position.Y += step * directionY;
                
                foreach (var checkCollider in colliderGroup.Entities)
                {
                    if (checkCollider == entity)
                        continue;
                    
                    ref var checkColliderTransform = ref checkCollider.GetComponent<TransformComponent>();
                    ref var checkColliderCollider = ref checkCollider.GetComponent<ColliderComponent>();

                    var checkColliderRect = EntityUtility.GetEntityCollisionRect(checkCollider);
                    var entityRect = EntityUtility.GetEntityCollisionRect(entity);
                    
                    var intersect = Rectangle.Intersect(entityRect, checkColliderRect);

                    if (intersect.Height <= 0)
                        continue;

                    var offset = directionY * -1f;
                    transform.Position.Y += intersect.Height * offset;
                    physics.IsFalling = false;
                    physics.Velocity.Y = 0;

                    if (checkColliderCollider.EventType != ColliderEventType.None)
                    {
                        if (checkColliderCollider.EventType == ColliderEventType.MovingPlatform)
                        {
                            ref var movingPlatform = ref checkCollider.GetComponent<MovingPlatformComponent>();
                            movingPlatform.EntityOnPlatform = entity;
                            physics.OnMovingPlatform = checkCollider;
                        }
                        else
                        {
                            entity.TryAddComponent(new ColliderEventComponent()
                            {
                                EventType = checkColliderCollider.EventType,
                                CollidedWith = checkCollider,
                            });
                        }
                    }
                }
            }
            
            while (movement.X > 0)
            {
                var step = moveStep;
                if (step > movement.X)
                    step = movement.X;

                movement.X -= step;
                physics.MoveAmount.X -= step * directionX;
                transform.Position.X += step * directionX;
                
                if (physics.OnMovingPlatform.IsAlive)
                    continue;

                foreach (var checkCollider in colliderGroup.Entities)
                {
                    if (checkCollider == entity)
                        continue;
                    
                    ref var checkColliderTransform = ref checkCollider.GetComponent<TransformComponent>();
                    ref var checkColliderCollider = ref checkCollider.GetComponent<ColliderComponent>();

                    var checkColliderRect = EntityUtility.GetEntityCollisionRect(checkCollider);
                    var entityRect = EntityUtility.GetEntityCollisionRect(entity);
                    
                    var intersect = Rectangle.Intersect(entityRect, checkColliderRect);

                    if (intersect.Width <= 0)
                        continue;

                    var offset = directionX * -1f;
                    transform.Position.X += intersect.Width * offset;

                    if (checkColliderCollider.EventType != ColliderEventType.None)
                    {
                        entity.TryAddComponent(new ColliderEventComponent()
                        {
                            EventType = checkColliderCollider.EventType,
                            CollidedWith = checkCollider,
                        });
                    }
                }
            }
            
            // if (prevPosition.ToVector2I().Y == prevPosition.ToVector2I().Y)
            //     physics.IsFalling = false;
        } // CollisionMovement
    }
}
