using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using System.Diagnostics;


namespace SDL_2_Test.engine
{
    public static class Movement
    {

        public static void Update(double elapsed, float x, float y)
        {
            Entity entity = EntityManager.GetPlayer();
            var isGrounded = entity.Grounded();
            var keyArray = Variables.KeyArray;

            //movement x and y

        }
    }
}





