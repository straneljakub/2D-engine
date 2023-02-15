using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SDL_2_Test.engine
{
    public static class Textures
    {
        public static SDL.SDL_Color FruitColor = new SDL.SDL_Color
        {
            r = 237,
            g = 2,
            b = 2,
            a = 255
        };

        public static SDL.SDL_Color StalkColor = new SDL.SDL_Color
        {
            r = 117,
            g = 86,
            b = 1,
            a = 255
        };

        public static SDL.SDL_Color BodyColor = new SDL.SDL_Color
        {
            r = 0,
            g = 0,
            b = 255,
            a = 255
        };

        public static SDL.SDL_Color SkinColor = new SDL.SDL_Color
        {
            r = 230,
            g = 197,
            b = 14,
            a = 255
        };

        public static SDL.SDL_Color ObstacleColor = new SDL.SDL_Color
        {
            r = 44,
            g = 242,
            b = 70,
            a = 255
        };

        public static (SDL.SDL_FRect, SDL.SDL_Color)[] PlayerShape()
        {
            (SDL.SDL_FRect, SDL.SDL_Color)[] shape = new (SDL.SDL_FRect, SDL.SDL_Color)[6];

            var head = new SDL.SDL_FRect
            {
                x = 315,
                y = 200,
                w = 10,
                h = 10
            };

            var body = new SDL.SDL_FRect
            {
                x = 307,
                y = 210,
                w = 26,
                h = 20
            };

            var leftArm = new SDL.SDL_FRect
            {
                x = 300,
                y = 210,
                w = 7,
                h = 15
            };

            var rightArm = new SDL.SDL_FRect
            {
                x = 333,
                y = 210,
                w = 7,
                h = 15
            };

            var leftLeg = new SDL.SDL_FRect
            {
                x = 307,
                y = 230,
                w = 7,
                h = 10
            };


            var rightLeg = new SDL.SDL_FRect
            {
                x = 326,
                y = 230,
                w = 7,
                h = 10
            };

            shape[0] = (head, SkinColor);
            shape[1] = (body, BodyColor);
            shape[2] = (leftArm, SkinColor);
            shape[3] = (rightArm, SkinColor);
            shape[4] = (leftLeg, SkinColor);
            shape[5] = (rightLeg, SkinColor);

            return shape;
        }

        public static (SDL.SDL_FRect, SDL.SDL_Color)[] ObstacleShape(float x, float y)
        {
            var shape = new (SDL.SDL_FRect, SDL.SDL_Color)[1];
            var body = new SDL.SDL_FRect
            {
                x = 0,
                y = Variables.LevelHeight - 50,
                w = Variables.LevelWidth,
                h = 50
            };
            shape[0] = (body, ObstacleColor);

            return shape;
        }

        public static (SDL.SDL_FRect, SDL.SDL_Color)[] FruitShape(float x, float y)
        {
            var shape = new (SDL.SDL_FRect, SDL.SDL_Color)[2];

            var stalk = new SDL.SDL_FRect
            {
                x = x + 8,
                y = y,
                w = 4,
                h = 4
            };

            var body = new SDL.SDL_FRect
            {
                x = x,
                y = y + 4,
                w = 20,
                h = 16
            };

            shape[0] = (stalk, StalkColor);
            shape[1] = (body, FruitColor);

            return shape;
        }
    }
}
