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
        public static int Index = 0;

        public static int AddAsset(string path)
        {
            IntPtr asset = SDL_image.IMG_Load(path);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Variables.Renderer, asset);
            if (texture == IntPtr.Zero) 
                return -1;
            AssetsArray[Index] = texture;

            return Index++;
        }

        public static void Animate(double elapsed)
        {
            Main.Animations(elapsed);
        }
    }

    
}
