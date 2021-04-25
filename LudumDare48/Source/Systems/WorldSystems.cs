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
                    transform.Position = player.RespawnPosition;
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
                        drawable.FlipType = SpriteFlipType.Horizontal;
                        break;

                    case MovementType.Right:
                        physics.Acceleration.X += physics.MoveSpeed;
                        physics.Velocity.X = 0;
                        drawable.FlipType = SpriteFlipType.None;
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
    }
}
