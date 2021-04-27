using System;
using System.IO;
using System.Numerics;
using ElementEngine;
using ElementEngine.ECS;
using System.Collections.Generic;

namespace LudumDare48
{
    public class RecordingData
    {
        public string Asset;
        public int Index;
    }
    
    public static class LevelGenerator
    {
        public static List<RecordingData> Recordings = new List<RecordingData>()
        {
            new RecordingData()
            {
                Asset = "Forecast.ogg",
                Index = 1,
            },
            
            new RecordingData()
            {
                Asset = "One Missed Message.ogg",
                Index = 2,
            },
            
            new RecordingData()
            {
                Asset = "First Message.ogg",
                Index = 3,
            },
            
            new RecordingData()
            {
                Asset = "Car Broke Down.ogg",
                Index = 4,
            },
            
            new RecordingData()
            {
                Asset = "No Permit.ogg",
                Index = 5,
            },
            
            new RecordingData()
            {
                Asset = "Farewell.ogg",
                Index = 6,
            },
        };
        
        public static void GenerateLevel(GameStatePlay gameState)
        {
            var rng = new Random();
            var platformsPerRecording = 10;
            var platforms = (platformsPerRecording * Recordings.Count) + platformsPerRecording;
            var jumpHeight = 100;
            var jumpLength = 200;
            var recordingIndex = 0;
            
            var prevPlatformPosition = Vector2.Zero;
            var nextPlatformPosition = new Vector2(25, 500);
            var prevMovingPlatform = false;
            
            for (var i = 0; i < platforms; i++)
            {
                var platform = EntityBuilder.CreatePlatform(nextPlatformPosition, PlatformType.Normal);
                ref var drawable = ref platform.GetComponent<DrawableMaskComponent>();
                
                var drawRect = EntityUtility.GetEntityDrawRect(platform);
                if (drawRect.Bottom > gameState.DeathHeight)
                    gameState.DeathHeight = drawRect.Bottom;
                
                var platformSize = drawable.AtlasRect.SizeF * drawable.Scale;
                var verticalDirection = rng.Next(0, 10) < 5 ? -1 : 1;
                var platformOffset = new Vector2(platformSize.X + jumpLength, verticalDirection * (platformSize.Y + jumpHeight));
                var recordingPlatform = false;
                
                if (i > 0 && i % platformsPerRecording == 0 && recordingIndex < Recordings.Count) {
                    var data = Recordings[recordingIndex];
                    var recording = EntityBuilder.CreateRecording(data.Asset, drawRect.LocationF, drawRect.LocationF + new Vector2(drawRect.Width / 2 - 100, -50f), recordingIndex == Recordings.Count - 1);
                    recordingIndex += 1;
                    drawable.Color = Veldrid.RgbaFloat.Green;
                    recordingPlatform = true;
                }
                
                if (i > 3 && rng.Next(0, 10) < 3)
                {
                    var secondVerticalDirection = prevPlatformPosition.Y < nextPlatformPosition.Y ? 1 : -1;
                    
                    var secondPlatformPosition = prevPlatformPosition
                        + new Vector2(
                            (platformSize.X + jumpLength) / 2,
                            ((platformSize.Y + jumpHeight) / 2) * secondVerticalDirection);
                    
                    EntityBuilder.CreatePlatform(secondPlatformPosition, PlatformType.Death);
                    prevMovingPlatform = false;
                }
                else if (i > 3 && rng.Next(0, 10) < 3 && !prevMovingPlatform && !recordingPlatform)
                {
                    var endPosition = nextPlatformPosition + new Vector2(platformOffset.X * 2f, 0);
                    
                    platform.TryAddComponent(new MovingPlatformComponent()
                    {
                        StartPosition = nextPlatformPosition,
                        EndPosition = endPosition,
                        Destination = endPosition,
                        MoveSpeed = 200,
                        BaseCooldown = 2f,
                        Cooldown = 2f,
                    });
                    
                    ref var collider = ref platform.GetComponent<ColliderComponent>();
                    collider.EventType = ColliderEventType.MovingPlatform;
                    
                    nextPlatformPosition = endPosition;
                    prevMovingPlatform = true;
                }
                else
                {
                    prevMovingPlatform = false;
                }
                
                prevPlatformPosition = nextPlatformPosition;
                nextPlatformPosition += platformOffset;
            }
            
            gameState.DeathHeight += 500;
            
        } // GenerateLevel
    } // LevelGenerator
}
