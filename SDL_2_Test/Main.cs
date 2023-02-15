﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL_2_Test.engine;
using SDL2;

namespace SDL_2_Test
{
    public static class Main
    {
        public static void Input(byte[] keyArray, Entity entity, double elapsed, bool isGrounded, float x, float y)
        {
            if (entity.GetType() == typeof(Player))
            {
                if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1 && keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
                    ;
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1 && isGrounded)
                    x -= (float)(Variables.PlayerSpeed * elapsed);
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1 && isGrounded)
                    x += (float)(Variables.PlayerSpeed * elapsed);
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
                    x -= (float)(Variables.PlayerSpeedAir * elapsed);
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
                    x += (float)(Variables.PlayerSpeedAir * elapsed);


                //jump
                if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1 && isGrounded)
                {
                    entity.SetForce(0, (float)(entity.GetMass() * 9.8) * -10);
                    entity.SetVelocity(40);
                    entity.SetHitbox(0, -1);
                }
            }

            entity.SetHitbox(x, y);
        }
        public static bool CollisionResolve(Entity entity, Entity collider)
        {
            if (collider.GetLeft() <= entity.GetLeft() && collider.GetRight() >= entity.GetRight() &&
                collider.GetTop() <= entity.GetTop() && collider.GetBottom() >= entity.GetBottom())
            {
                if (entity.GetType() == typeof(Fruit))
                {
                    var hitbox = entity.GetHitbox();
                    entity.SetHitbox(-hitbox.x, -hitbox.y);
                    entity.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                    return true;
                }
            }

            if (entity.GetType() == typeof(Fruit) && collider.GetType() == typeof(Fruit))
            {
                return true;
            }


            if (entity.GetType() == typeof(Player) && collider.GetType() == typeof(Fruit))
            {
                var hitbox = collider.GetHitbox();
                collider.SetHitbox(-hitbox.x, -hitbox.y);
                collider.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                collider.SetVelocity(0);
                Variables.Score += 1;
                return true;
            }

            if (entity.GetType() == typeof(Fruit) && collider.GetType() == typeof(Player))
            {
                var hitbox = entity.GetHitbox();
                entity.SetHitbox(-hitbox.x, -hitbox.y);
                entity.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                entity.SetVelocity(0);
                Variables.Score += 1;
                return true;
            }

            return false;
        }
    }
}