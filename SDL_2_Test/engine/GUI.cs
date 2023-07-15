using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SDL_2_Test.engine
{
    public static class GUI
    {
        public static List<Button> Buttons = new List<Button>();
        public static void DrawMenu()
        {
            SDL.SDL_SetRenderDrawColor(Variables.Renderer, 135, 206, 235, 255);
            SDL.SDL_RenderClear(Variables.Renderer);
            foreach (var button in Buttons)
            {
                DrawButton(button);
            }
            SDL.SDL_RenderPresent(Variables.Renderer);
        }

        public static void DrawButton(Button b)
        {
            SDL.SDL_RenderCopy(Variables.Renderer, Assets.AssetsArray[b.buttonTexture], IntPtr.Zero, ref b.rect);

            var textRect = new SDL.SDL_Rect
            {
                x = b.rect.x + (b.rect.w / 5) / 2,
                y = b.rect.y + (b.rect.h / 8) / 2,
                h = b.rect.h - b.rect.h / 8,
                w = b.rect.w - b.rect.w / 5
            };
            SDL.SDL_RenderCopy(Variables.Renderer, Assets.AssetsArray[b.textTexture], IntPtr.Zero, ref textRect);
        }
        public static void PollMenuEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                Variables.Running = true;
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        {
                            if (ButtonClicked(Variables.PlayButton, e))
                            {
                                Variables.Running = true;
                                break;
                            }
                            else if (ButtonClicked(Variables.QuitButton, e))
                            {
                                Program.CleanUp();
                                Variables.Quit = true;
                                break;
                            } 
                            else if (ButtonClicked(Variables.SaveButton, e)) {
                                Level.SaveLevel();
                                break;
                            }
                        }
                        break;

                }
            }
        }

        public static bool ButtonClicked(Button b, SDL.SDL_Event e)
        {
            var r = b.rect;
            if (r.x <= e.button.x && r.x + r.w >= e.button.x &&
                           r.y <= e.button.y && r.y + r.h >= e.button.y)
            {
                return true;
            }
            return false;
        }
    }



    public class Button
    {
        public SDL2.SDL.SDL_Rect rect;
        string text;
        public int textTexture;
        public int buttonTexture;
        public bool clicked;

        public Button(int x, int y, int w, int h, string text, int buttonIndex)
        {
            this.rect = new SDL.SDL_Rect();
            this.rect.x = x;
            this.rect.y = y;
            this.rect.w = w;
            this.rect.h = h;
            this.text = text;
            this.buttonTexture = buttonIndex;
            this.clicked = false;
            this.textTexture = Assets.AddText(text);
            GUI.Buttons.Add(this);
        }
    }
}
