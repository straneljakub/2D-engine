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
        {
            Variables.Camera.x = EntityManager.GetPlayer().GetHitbox().x - Variables.ScreenWidth / 2;
        }
    }
}
