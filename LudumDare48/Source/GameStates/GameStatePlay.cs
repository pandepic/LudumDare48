using ElementEngine;
using ElementEngine.ECS;

namespace LudumDare48
{
    public class GameStatePlay : GameState
    {
        public Game Game;
        public Registry Registry;
        public SpriteBatch2D SpriteBatch;
        
        // ECS groups
        public Group DrawableGroup;
        
        public GameStatePlay(Game game)
        {
            Game = game;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            Registry = new Registry();
            
            DrawableGroup = Registry.RegisterGroup<TransformComponent, DrawableComponent>();
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Point);
            Systems.Render(DrawableGroup, SpriteBatch);
            SpriteBatch.End();
        }
    }
}
