using ElementEngine;

namespace LudumDare48
{
    public struct SpriteComponent
    {
        public Vector2I FrameSize;
        public Vector2I Size;
    }
    
    public struct SpriteAnimationComponent
    {
        public AnimationType Type;
        public int StartFrame;
        public int EndFrame;
        public float BaseFrameTime;
        public float CurrentFrameTime;
        public int CurrentFrame;
        public bool Loop;
    }
}