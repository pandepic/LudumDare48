using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.UI;
using ImGuiNET;

namespace LudumDare48
{
    public class DebugManager
    {
        private GameStatePlay _playState;
        private PrimitiveBatch _primitiveBatch;
        private bool _drawCollisionRects;
        
        private Veldrid.RgbaFloat _collisionRectColor = new Veldrid.RgbaFloat(1f, 0f, 0f, 0.5f);
        
        public DebugManager(GameStatePlay playState)
        {
            _playState = playState;
            _primitiveBatch = new PrimitiveBatch();
            
            IMGUIManager.Setup();
        }
        
        public void Update(GameTimer gameTimer)
        {
            IMGUIManager.Update(gameTimer);
        }
        
        public void Draw()
        {
            if (_drawCollisionRects)
            {
                _primitiveBatch.Begin(SamplerType.Point, _playState.Camera.GetViewMatrix());
                
                foreach (var entity in _playState.ColliderGroup.Entities)
                {
                    var rect = EntityUtility.GetEntityCollisionRect(entity);
                    _primitiveBatch.DrawFilledRect(rect, _collisionRectColor);
                }
                
                _primitiveBatch.End();
            }
            
            ImGui.Begin("Debug Menu", ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.Checkbox("Collision Rects", ref _drawCollisionRects);
            ImGui.End();
            
            IMGUIManager.Draw();
        }
    }
}