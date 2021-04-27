using ElementEngine;

namespace LudumDare48
{
    public class GameStateWin : GameState, IUIEventHandler
    {
        public Game Game;
        public SpriteBatch2D SpriteBatch;
        public UIMenu Menu;
        
        public GameStateWin(Game game)
        {
            Game = game;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            Menu = new UIMenu();
            Menu.Load("Win.xml", "Templates.xml");
            Menu.AddUIEventHandler(this);
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
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Point);
            Menu.Draw(SpriteBatch);
            SpriteBatch.End();
        }
        
        public void HandleUIEvent(UIMenu source, UIEventType type, UIWidget widget)
        {
            switch (widget.Name)
            {
                case "btnPlay":
                {
                    Game.SetGameState(GameStateType.Play);
                }
                break;
                
                case "btnExit":
                {
                    Game.Quit();
                }
                break;
            }
        }
    }
}