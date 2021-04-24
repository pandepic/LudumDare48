using System.Collections.Generic;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

using Rectangle = ElementEngine.Rectangle;

namespace LudumDare48
{
    public static partial class Systems
    {
        public static void ColliderEvents(Group group)
        {
            foreach (var entity in group.Entities)
            {
                ref var colliderEvent = ref entity.GetComponent<ColliderEventComponent>();
                
                switch (colliderEvent.EventType)
                {
                    case ColliderEventType.Death:
                    {
                        entity.TryAddComponent(new DeathTag());
                    }
                    break;
                }
                
                entity.RemoveComponent<ColliderEventComponent>();
            }
        }
    }
}
