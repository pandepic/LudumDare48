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
        private Entity _player;
        private PrimitiveBatch _primitiveBatch;
        private bool _drawCollisionRects;
        
        private Veldrid.RgbaFloat _collisionRectColor = new Veldrid.RgbaFloat(1f, 0f, 0f, 0.5f);
        
        public DebugManager(GameStatePlay playState, Entity player)
        {
            _playState = playState;
            _player = player;
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
            
            ref var transform = ref _player.GetComponent<TransformComponent>();
            ref var physics = ref _player.GetComponent<PhysicsComponent>();
            
            ImGui.Begin("Debug Menu", ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.Text($"Position: {transform.TransformedPosition}");
            ImGui.Text($"Velocity: {physics.Velocity}");
            ImGui.Text($"Acceleration: {physics.Acceleration}");
            ImGui.Text($"IsFalling: {physics.IsFalling}");
            ImGui.Checkbox("Collision Rects", ref _drawCollisionRects);
            ImGui.End();
            
            IMGUIManager.Draw();
        }
    }
}