using System.IO;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;

namespace LudumDare48
{
    public class GameStatePlay : GameState
    {
        public const float GRAVITY = 1000f;
        public const int MOVE_STEP = 8;

        public float DeathHeight = 0f;
        public bool HasWon = false;

        public Game Game;
        public Registry Registry;
        public SpriteBatch2D SpriteBatch;
        public Camera2D Camera;
        public SpriteFont Font;
        public DebugManager DebugManager;

        public bool ShowDebug = false;

        // ECS groups
        public Group DrawableGroup;
        public Group DrawableMaskGroup;
        public Group DrawableOverlayGroup;
        public Group DrawableBackgroundGroup;
        public Group PhysicsGroup;
        public Group ColliderGroup;
        public Group ColliderEventGroup;
        public Group DeathGroup;
        public Group StartMovementGroup;
        public Group StopMovementGroup;
        public Group SpriteAnimationGroup;
        public Group PlayerAnimationGroup;
        public Group MovingPlatformGroup;
        public Group RecordingsGroup;

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
            Font = AssetManager.LoadSpriteFont("LatoBlack.ttf");
        }

        // called every time the state loads
        public override void Load()
        {
            HasWon = false;
            
            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.Window.Width, ElementGlobals.Window.Height));
            Camera.Zoom = 0.5f;

            Registry = new Registry();

            DrawableGroup = Registry.RegisterGroup<TransformComponent, DrawableComponent>();
            DrawableMaskGroup = Registry.RegisterGroup<TransformComponent, DrawableMaskComponent>();
            DrawableOverlayGroup = Registry.RegisterGroup<OverlayComponent>();
            DrawableBackgroundGroup = Registry.RegisterGroup<BackgroundComponent>();
            PhysicsGroup = Registry.RegisterGroup<TransformComponent, PhysicsComponent>();
            ColliderGroup = Registry.RegisterGroup<TransformComponent, ColliderComponent>();
            ColliderEventGroup = Registry.RegisterGroup<ColliderEventComponent>();
            DeathGroup = Registry.RegisterGroup<DeathTag>();
            StartMovementGroup = Registry.RegisterGroup<StartMovementComponent, PhysicsComponent, DrawableMaskComponent>();
            StopMovementGroup = Registry.RegisterGroup<StopMovementComponent, PhysicsComponent, DrawableMaskComponent>();
            SpriteAnimationGroup = Registry.RegisterGroup<SpriteAnimationComponent, SpriteComponent, DrawableMaskComponent>();
            PlayerAnimationGroup = Registry.RegisterGroup<PlayerAnimationComponent, SpriteAnimationComponent, SpriteComponent>();
            MovingPlatformGroup = Registry.RegisterGroup<MovingPlatformComponent>();
            RecordingsGroup = Registry.RegisterGroup<RecordingComponent>();
            
            EntityBuilder.Registry = Registry;
            
            Player = EntityBuilder.CreatePlayer(new Vector2(50, -100));

            EntityBuilder.CreateBackgrounds();

            LevelGenerator.GenerateLevel(this);
            DebugManager = new DebugManager(this, Player);
            
            SoundManager.SetVolume(0, 0.25f);
            SoundManager.SetVolume(1, 0.5f);
            SoundManager.Play("Open Your Mind (intense loop).ogg", 0, loop: true);
        }

        public override void Update(GameTimer gameTimer)
        {
            Systems.Physics(PhysicsGroup, ColliderGroup, gameTimer, GRAVITY, MOVE_STEP, DeathHeight);
            Systems.ColliderEvents(ColliderEventGroup);
            Systems.Death(DeathGroup);
            Systems.StartMovement(StartMovementGroup);
            Systems.StopMovement(StopMovementGroup);
            Systems.SpriteAnimation(SpriteAnimationGroup, gameTimer);
            Systems.PlayerAnimation(PlayerAnimationGroup);
            Systems.MovingPlatforms(MovingPlatformGroup, gameTimer);
            Systems.Recordings(RecordingsGroup, Player, this);

            // process removal queues for entities and components
            Registry.SystemsFinished();

            var playerRect = EntityUtility.GetEntityDrawRect(Player);
            Camera.Center(playerRect.Center);

            if (ShowDebug)
                DebugManager.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            var playerRect = EntityUtility.GetEntityDrawRect(Player);

            Systems.RenderBackground(DrawableBackgroundGroup, Camera);

            SpriteBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
            Systems.Render(DrawableGroup, SpriteBatch);
            SpriteBatch.End();

            Systems.RenderMask(DrawableMaskGroup, Camera);
            Systems.RenderOverlay(DrawableOverlayGroup, Camera);

            SpriteBatch.Begin(SamplerType.Point);
            //SpriteBatch.DrawText(Font, playerRect.Location.ToString(), new Vector2(25), Veldrid.RgbaByte.White, 32, 1);
            SpriteBatch.End();

            if (ShowDebug)
                DebugManager.Draw();
        }

        public override void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            switch (controlName)
            {
                case "MoveUp":
                if (state == GameControlState.Pressed)
                {
                    ref var physics = ref Player.GetComponent<PhysicsComponent>();

                    if (!physics.IsFalling)
                    {
                        Player.TryAddComponent(new StartMovementComponent()
                        {
                            MovementType = MovementType.Jump,
                        });
                    }
                }
                break;

                case "MoveDown":
                break;

                case "MoveLeft":
                if (state == GameControlState.Pressed)
                {
                    Player.TryAddComponent(new StartMovementComponent()
                    {
                        MovementType = MovementType.Left,
                    });
                }
                else if (state == GameControlState.Released)
                {
                    Player.TryAddComponent(new StopMovementComponent()
                    {
                        MovementType = MovementType.Left,
                    });
                }
                break;

                case "MoveRight":
                if (state == GameControlState.Pressed)
                {
                    Player.TryAddComponent(new StartMovementComponent()
                    {
                        MovementType = MovementType.Right,
                    });
                }
                else if (state == GameControlState.Released)
                {
                    Player.TryAddComponent(new StopMovementComponent()
                    {
                        MovementType = MovementType.Right,
                    });
                }
                break;

                case "Debug":
                if (state == GameControlState.Pressed)
                {
                    ShowDebug = !ShowDebug;
                }
                break;
            }
        } // HandleGameControl
        
        public void Win()
        {
            Game.SetGameState(GameStateType.Win);
        }

    } // GameStatePlay
}
