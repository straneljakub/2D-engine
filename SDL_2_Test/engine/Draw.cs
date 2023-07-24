using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SDL2;

namespace SDL_2_Test.engine
{
    public class Draw
    {
        // Fonts
        public static IntPtr Arial = SDL_ttf.TTF_OpenFont(Path.Combine(AppContext.BaseDirectory, "arial.ttf"), 26);
        // SDL Colors
        public static SDL.SDL_Color Black = new SDL.SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
        // Fps texture creation
        public static IntPtr SurfaceMessage = SDL_ttf.TTF_RenderText_Solid(Arial, "FPS: " + Variables.CurrentFps, Black);
        public static IntPtr Message = SDL.SDL_CreateTextureFromSurface(Variables.Renderer, SurfaceMessage);
        public static SDL.SDL_Rect MessageRect = new SDL.SDL_Rect() { x = 10, y = 10, w = 180, h = 30 };
        public static bool IsFpsInvalid = false;
        public static void DrawFps()
        {

            if (IsFpsInvalid)
            {
                SurfaceMessage = SDL_ttf.TTF_RenderText_Solid(Arial, "FPS: " + Variables.CurrentFps + " Score : " + Variables.Score, Black);

                Message = SDL.SDL_CreateTextureFromSurface(Variables.Renderer, SurfaceMessage);

                IsFpsInvalid = false;
            }

            if (SDL.SDL_RenderCopy(Variables.Renderer, Message, IntPtr.Zero, ref MessageRect) < 0)
            {
                Console.WriteLine("FPS rendering failed" + SDL.SDL_GetError());
            }

            // Don't forget to free your surface and texture


        }

        public static void DrawText(int spriteIndex)
        {
            SDL.SDL_RenderCopy(Variables.Renderer, Message, IntPtr.Zero, Assets.AssetsArray[spriteIndex]);
        }
    }
}
