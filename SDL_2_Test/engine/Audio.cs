using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using System.Diagnostics;

namespace SDL_2_Test.engine
{
    public static class Audio
    {
        public static void PlayEffect(IntPtr effect)
        {
            SDL_mixer.Mix_PlayChannel(-1, effect, 0);
        }

        public static void PlayMusic(IntPtr music)
        {
            SDL_mixer.Mix_PlayChannel(-1, music, -1);
            Debug.WriteLine(SDL_mixer.Mix_GetError());
        }
    }
}
