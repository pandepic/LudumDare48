using System.Collections.Generic;
using ElementEngine;

namespace LudumDare48
{
    public enum GameStateType
    {
        Menu,
        Play,
        Win,
        Loading,
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
            
            SetupWindow(windowRect, "1 Missed Message", null, vsync);
            SetupAssets("Content");
            
            ClearColor = Veldrid.RgbaFloat.White;
            
            InputManager.LoadGameControls();
            Window.Resizable = false;

            GameStates.Add(GameStateType.Menu, new GameStateMenu(this));
            GameStates.Add(GameStateType.Play, new GameStatePlay(this));
            GameStates.Add(GameStateType.Win, new GameStateWin(this));
            GameStates.Add(GameStateType.Loading, new GameStateLoading(this));
            
            SetGameState(GameStateType.Menu);
        }

        public void SetGameState(GameStateType type)
        {
            if (GameStates.TryGetValue(type, out var state))
                SetGameState(state);
        }
    }
}
