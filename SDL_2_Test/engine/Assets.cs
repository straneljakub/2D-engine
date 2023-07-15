using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;


namespace SDL_2_Test.engine
{
    public static class Assets
    {
        public static IntPtr[] AssetsArray = new IntPtr[1024];
        public static List<string> AssetsPaths = new List<string>();
        public static int Index = 0;

        public static int AddAsset(string path)
        {
            AssetsPaths.Add(path);
            var extension = path.Split('.').Last();
            if(extension == "jpg" || extension == "png")
            {
                IntPtr asset = SDL_image.IMG_Load(path);
                IntPtr texture = SDL.SDL_CreateTextureFromSurface(Variables.Renderer, asset);
                if (texture == IntPtr.Zero)
                    return -1;
                AssetsArray[Index] = texture;

                return Index++;
            } else if (extension == "mp3")
            {
                IntPtr asset = SDL_mixer.Mix_LoadMUS(path);
                if (asset == IntPtr.Zero)
                    return -1;
                AssetsArray[Index] = asset;

                return Index++;
            } else if (extension == "wav") {
                IntPtr asset = SDL_mixer.Mix_LoadWAV(path);
                if (asset == IntPtr.Zero)
                    return -1;
                AssetsArray[Index] = asset;

                return Index++;
            }

            return -1;
        }

        public static int AddText(string text)
        {
            IntPtr arial = SDL_ttf.TTF_OpenFont(Path.Combine(AppContext.BaseDirectory, "arial.ttf"), 26);
            SDL.SDL_Color white = new SDL.SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
            IntPtr surfaceText = SDL_ttf.TTF_RenderText_Solid(arial, text, white);
            IntPtr textTexture = SDL.SDL_CreateTextureFromSurface(Variables.Renderer, surfaceText);
            
            if (textTexture == IntPtr.Zero)
                return -1;
            AssetsArray[Index] = textTexture;

            return Index++;
        }

        public static void Animate(double elapsed)
        {
            Main.Animations(elapsed);
        }
    }

    
}
