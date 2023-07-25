using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SDL_2_Test.engine
{
    public static class Variables
    {
        public readonly static int ScreenWidth = 1000;
        public readonly static int ScreenHeight = 600;
        public readonly static int LevelWidth = 1000;
        public readonly static int LevelHeight = 600;
        public static SDL.SDL_FRect Camera = new SDL.SDL_FRect
        {
            x = 0,
            y = 0,
            w = 600,
            h = 600
        };

        public static IntPtr Window;
        public static IntPtr Renderer;


        // Player speed in pixels per second
        public static int PlayerSpeed = 350;
        public static int PlayerSpeedAir = 200;

        public static int Meter = 20;
        public static double GravitationalPull = 9.8;

        public static bool Running = false;
        public static bool Quit = false;
        // Nanoseconds
        public static int FrameDuration = 8333333;
        //Current framerate
        public static int CurrentFps = 0;

        // Obstacles array
        public static List<Entity> Entities = new List<Entity>();



        public static Random Random = new Random();


        //Score
        public static int Score = 0;

        //Key Array
        public static byte[] KeyArray = new byte[512];


        //Menu buttons
        public static Button PlayButton;
        public static Button QuitButton;
        public static Button SaveButton;
        public static Boolean DrawFps = true;
        public static Boolean GameOver = false;
        public static string CurrentLevel = "";
    }
}
