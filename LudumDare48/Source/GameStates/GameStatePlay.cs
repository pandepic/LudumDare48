using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

namespace LudumDare48
{
    public class GameStatePlay : GameState
    {
        public const float GRAVITY = 100f;
        public const float MOVE_STEP = 8f;
        
        public Game Game;
        public Registry Registry;
        public SpriteBatch2D SpriteBatch;
        public Camera2D Camera;
        
        // ECS groups
        public Group DrawableGroup;
        public Group PhysicsGroup;
        public Group ColliderGroup;
        public Group ColliderEventGroup;
        public Group DeathGroup;
        public Group MovementGroup;
        
        // Special entities
        public Entity Player;
        
        public GameStatePlay(Game game)
        {
            Game = game;
        }

        // only called once ever
        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            Registry = new Registry();
            
            DrawableGroup = Registry.RegisterGroup<TransformComponent, DrawableComponent>();
            PhysicsGroup = Registry.RegisterGroup<TransformComponent, PhysicsComponent>();
            ColliderGroup = Registry.RegisterGroup<TransformComponent, PhysicsComponent, ColliderComponent>();
            ColliderEventGroup = Registry.RegisterGroup<ColliderEventComponent>();
            DeathGroup = Registry.RegisterGroup<DeathTag>();
            MovementGroup = Registry.RegisterGroup<MovementComponent, PhysicsComponent, DrawableComponent>();
            
            EntityBuilder.Registry = Registry;
            
            Player = EntityBuilder.CreatePlayer(new Vector2(50, 50));
        }

        // called every time the state loads
        public override void Load()
        {
            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.Window.Width, ElementGlobals.Window.Height));
        }
        
        public override void Update(GameTimer gameTimer)
        {
            Systems.Physics(PhysicsGroup, ColliderGroup, gameTimer, GRAVITY, MOVE_STEP);
            Systems.ColliderEvents(ColliderEventGroup);
            Systems.Death(DeathGroup);
            
            // process removal queues for entities and components
            Registry.SystemsFinished();
            
            var playerRect = EntityUtility.GetEntityDrawRect(Player);
            //Camera.Center(playerRect.Center);
        }
        
        public override void Draw(GameTimer gameTimer)
        {
            var test = new Texture2D(50, 50, Veldrid.RgbaByte.Red);
            var rng = new System.Random();
            
            SpriteBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
            Systems.Render(DrawableGroup, SpriteBatch);
            SpriteBatch.End();
        }
        
        public override void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            switch (controlName)
            {
                case "MoveUp":
                {
                    Player.TryAddComponent(new MovementComponent()
                    {
                        MovementType = MovementType.Jump,
                    });
                }
                break;
                
                case "MoveDown":
                {
                }
                break;
                
                case "MoveLeft":
                {
                    Player.TryAddComponent(new MovementComponent()
                    {
                        MovementType = MovementType.Left,
                    });
                }
                break;
                
                case "MoveRight":
                {
                    Player.TryAddComponent(new MovementComponent()
                    {
                        MovementType = MovementType.Right,
                    });
                }
                break;
            }
        }
        
    } // GameStatePlay
}
