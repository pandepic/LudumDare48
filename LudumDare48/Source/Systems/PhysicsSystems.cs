using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        public static void Physics(Group physicsGroup, Group colliderGroup, GameTimer gameTimer, float gravity, float moveStep)
        {
            foreach (var entity in physicsGroup.Entities)
            {
                ref var transform = ref entity.GetComponent<TransformComponent>();
                ref var physics = ref entity.GetComponent<PhysicsComponent>();

                physics.Velocity += physics.Acceleration * gameTimer.DeltaS;
                physics.Velocity.Y += gravity * gameTimer.DeltaS;

                if (physics.Velocity.X > physics.MaxSpeed.X)
                    physics.Velocity.X = physics.MaxSpeed.X;
                if (physics.Velocity.Y > physics.MaxSpeed.Y)
                    physics.Velocity.Y = physics.MaxSpeed.Y;

                if (!entity.HasComponent<ColliderComponent>())
                    transform.Position += physics.Velocity * gameTimer.DeltaS;
                else
                    CollisionMovement(entity, ref transform, ref physics, colliderGroup, gameTimer, moveStep);
            }

        } // Physics

        private static void CollisionMovement(Entity entity, ref TransformComponent transform, ref PhysicsComponent physics, Group colliderGroup, GameTimer gameTimer, float moveStep)
        {
            ref var collider = ref entity.GetComponent<ColliderComponent>();
            var entityRect = new Rectangle(collider.CollisionRect.Location.ToVector2() + transform.TransformedPosition, collider.CollisionRect.SizeF);

            var directionY = -1f;
            if (physics.Velocity.Y > 0)
                directionY = 1f;

            var directionX = -1f;
            if (physics.Velocity.X > 0)
                directionX = 1f;

            var movement = physics.Velocity * gameTimer.DeltaS;
            if (movement.X < 0)
                movement.X *= -1f;
            if (movement.Y < 0)
                movement.Y *= -1f;

            while (movement.Y > 0)
            {
                var step = moveStep;
                if (step > movement.Y)
                    step = movement.Y;

                movement.Y -= step;

                transform.Position.Y += step * directionY;
                physics.IsFalling = true;

                foreach (var checkCollider in colliderGroup.Entities)
                {
                    ref var checkColliderTransform = ref entity.GetComponent<TransformComponent>();
                    ref var checkColliderCollider = ref entity.GetComponent<ColliderComponent>();

                    var checkColliderRect = new Rectangle(
                        checkColliderCollider.CollisionRect.Location.ToVector2() + checkColliderTransform.TransformedPosition,
                        checkColliderCollider.CollisionRect.SizeF);

                    var intersect = Rectangle.Intersect(entityRect, checkColliderRect);

                    if (intersect.Height <= 0)
                        continue;

                    var offset = directionY * -1f;
                    transform.Position.Y += intersect.Height * offset;
                    physics.IsFalling = false;

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

            while (movement.X > 0)
            {
                var step = moveStep;
                if (step > movement.X)
                    step = movement.X;

                movement.X -= step;

                transform.Position.X += step * directionY;

                foreach (var checkCollider in colliderGroup.Entities)
                {
                    ref var checkColliderTransform = ref entity.GetComponent<TransformComponent>();
                    ref var checkColliderCollider = ref entity.GetComponent<ColliderComponent>();

                    var checkColliderRect = new Rectangle(
                        checkColliderCollider.CollisionRect.Location.ToVector2() + checkColliderTransform.TransformedPosition,
                        checkColliderCollider.CollisionRect.SizeF);

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
        } // CollisionMovement
    }
}