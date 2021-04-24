using System.Collections.Generic;
using ElementEngine;

namespace LudumDare48
{
    public enum GameStateType
    {
        Menu,
        Play,
    }
    
    public class Game : BaseGame
    {
        public Dictionary<GameStateType, GameState> GameStates { get; set; } = new Dictionary<GameStateType, GameState>();

        public override void Load()
        {
            SettingsManager.LoadFromPath("Settings.xml");
            
            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = SettingsManager.GetSetting<int>("Window", "Width"),
                Height = SettingsManager.GetSetting<int>("Window", "Height")
            };
            
            var vsync = false;
#if DEBUG
            vsync = false;
#endif
            
            SetupWindow(windowRect, "LudumDare48", null, vsync);
            SetupAssets("Content");
            
            ClearColor = Veldrid.RgbaFloat.CornflowerBlue;
            
            InputManager.LoadGameControls();
            Window.Resizable = false;
            
            GameStates.Add(GameStateType.Menu, new GameStateMenu(this));
            GameStates.Add(GameStateType.Play, new GameStatePlay(this));
            
            SetGameState(GameStateType.Play);
        }
        
        public void SetGameState(GameStateType type)
        {
            if (GameStates.TryGetValue(type, out var state))
                SetGameState(state);
        }
    }
}
