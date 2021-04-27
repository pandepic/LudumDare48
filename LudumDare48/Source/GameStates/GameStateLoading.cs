using ElementEngine;

namespace LudumDare48
{
    public class GameStateLoading : GameState
    {
        public Game Game;
        public SpriteBatch2D SpriteBatch;
        public UIMenu Menu;
        
        public float DummyTime = 0.2f;
        
        public GameStateLoading(Game game)
        {
            Game = game;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            Menu = new UIMenu();
            Menu.Load("Loading.xml", "Templates.xml");
        }

        public override void Load()
        {
            Game.ClearColor = Veldrid.RgbaFloat.Black;
            Menu.EnableInput();
        }

        public override void Unload()
        {
            Menu.DisableInput();
        }

        public override void Update(GameTimer gameTimer)
        {
            Menu.Update(gameTimer);
            
            DummyTime -= gameTimer.DeltaS;
            
            if (DummyTime <= 0f)
            {
                Game.SetGameState(GameStateType.Play);
            }
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Point);
            Menu.Draw(SpriteBatch);
            SpriteBatch.End();
        }
    }
}