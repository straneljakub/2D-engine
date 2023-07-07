using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL_2_Test.engine
{
    public static class Camera
    {
        public static void Update()
        { float x = EntityManager.GetPlayer().GetHitbox().x;
            if (x < Variables.ScreenWidth / 2)
            {
                Variables.Camera.x = 0;
            } else if (x > (Variables.LevelWidth - Variables.ScreenWidth / 2))
            {
                Variables.Camera.x = Variables.LevelWidth - Variables.ScreenWidth;
            } else
            {
                Variables.Camera.x = x - Variables.ScreenWidth / 2;
            }
        }
    }
}
