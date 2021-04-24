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
                entity.RemoveComponent<DeathTag>();
                group.Registry.DestroyEntity(entity);
            }
        }
        
        public static void MovementComponent(Group group)
        {
            foreach (var entity in group.Entities)
            {
                ref var movement = ref entity.GetComponent<MovementComponent>();
                ref var physics = ref entity.GetComponent<PhysicsComponent>();
                
                switch (movement.MovementType)
                {
                    case MovementType.Left:
                        physics.Acceleration.X = physics.MoveSpeed;
                        break;
                    
                    case MovementType.Right:
                        physics.Acceleration.X = -physics.MoveSpeed;
                        break;
                    
                    case MovementType.Jump:
                        physics.Acceleration.Y = -physics.JumpSpeed;
                        break;
                }
                
                entity.RemoveComponent<MovementComponent>();
            }
        }
    }
}
