using SDL2;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace SDL_2_Test.engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Setup();

            Stopwatch sw;

            // Nanoseconds
            double elapsed = 1;
            // Nanoseconds
            float nanosecondsSum = 0;
            int frameSum = 0;
            int sleepMiliseconds;


            int size;

            while (!Variables.Running)
            {
                GUI.PollMenuEvents();
                MainProgram.MenuLoop();
                GUI.DrawMenu();
                if (Variables.Quit)
                {
                    break;
                }
                while (Variables.Running)
                {
                    // Start counting
                    sw = Stopwatch.StartNew();




                    // Main Game Loop Functions Start
                    PollEvents();
                    MainProgram.GameLoop();
                    IntPtr state = SDL.SDL_GetKeyboardState(out size);
                    Marshal.Copy(state, Variables.KeyArray, 0, size);
                    Physics.Update(elapsed);
                    Camera.Update();
                    Assets.Animate(elapsed);
                    Render();
                    // Main Game Loop Functions End



                    // Stop counting
                    sw.Stop();

                    // Nanoseconds
                    elapsed = sw.Elapsed.TotalMilliseconds * 1000000;

                    // Frame + 1
                    frameSum += 1;

                    // Draw FPS every second
                    nanosecondsSum += Variables.FrameDuration;// elapsed;
                    if (nanosecondsSum >= 1000000000 && Variables.DrawFps == true)
                    {
                        Variables.CurrentFps = frameSum;
                        Draw.IsFpsInvalid = true;
                        nanosecondsSum = 0;
                        frameSum = 0;
                    }


                    // If time of 1 frame is shorther than frame duration, sleep

                    if (elapsed < Variables.FrameDuration)
                    {
                        sleepMiliseconds = (Variables.FrameDuration - (int)elapsed) / 1000000;
                        Thread.Sleep(sleepMiliseconds);
                        elapsed = Variables.FrameDuration;
                    }
                    if (!Variables.Running)
                    {
                        break;
                    }
                }
            }




            CleanUp();

            /// <summary>
            /// Setup all of the SDL resources we'll need to display a window.
            /// </summary>
            void Setup()
            {
                // Initilizes SDL.
                if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
                {
                    Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
                }


                if (SDL_ttf.TTF_Init() < 0)
                {
                    Console.WriteLine($"There was an issue initializing SDL_ttf. {SDL.SDL_GetError()}");
                }

                // Create a new window given a title, size, and passes it a flag indicating it should be shown.
                Variables.Window = SDL.SDL_CreateWindow(
                    "SDL .NET 6 Tutorial",
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    Variables.ScreenWidth,
                    Variables.ScreenHeight,
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

                if (Variables.Window == IntPtr.Zero)
                {
                    Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
                }



                // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
                Variables.Renderer = SDL.SDL_CreateRenderer(
                    Variables.Window,
                    -1,
                    SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

                if (Variables.Renderer == IntPtr.Zero)
                {
                    Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
                }

                var flags = SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG;
                if (SDL_image.IMG_Init(flags) < 0)
                {
                    Console.WriteLine($"There was an issue initializing SDL_image. {SDL.SDL_GetError()}");
                }

                SDL.SDL_SetRenderDrawBlendMode(Variables.Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);


                var mixerFlags = SDL_mixer.MIX_InitFlags.MIX_INIT_FLAC | SDL_mixer.MIX_InitFlags.MIX_INIT_MID | SDL_mixer.MIX_InitFlags.MIX_INIT_MP3 | SDL_mixer.MIX_InitFlags.MIX_INIT_OGG | SDL_mixer.MIX_InitFlags.MIX_INIT_MOD;
                if (SDL_mixer.Mix_Init(mixerFlags) < 0)
                {
                    Console.WriteLine($"There was an issue initializing SDL_mixer. {SDL.SDL_GetError()}");
                }

                if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 5, 2024) < 0)
                {
                    Debug.WriteLine($"There was an issue initializing SDL_mixer. {SDL.SDL_GetError()}");
                }


                MainProgram.Init();
            }



            /// Checks to see if there are any events to be processed.

            void PollEvents()
            {
                // Check to see if there are any events and continue to do so until the queue is empty.
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            Variables.Running = false;
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            switch (e.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_UP:

                                    break;
                                case SDL.SDL_Keycode.SDLK_DOWN:

                                    break;
                                case SDL.SDL_Keycode.SDLK_LEFT:

                                    break;
                                case SDL.SDL_Keycode.SDLK_RIGHT:

                                    break;
                            }
                            break;


                    }
                }
            }

            /// <summary>
            /// Renders to the window.
            /// </summary>
            void Render()
            {



                // Sets the color that the screen will be cleared with.
                SDL.SDL_SetRenderDrawColor(Variables.Renderer, 135, 206, 235, 255);

                // Clears the current render surface.
                SDL.SDL_RenderClear(Variables.Renderer);

                foreach (Entity entity in Variables.Entities)
                {
                    if (entity.GetType() == typeof(Player))
                    {
                        continue;
                    }
                    var sc = entity.GetComponent<SpriteComponent>();
                    var t = Assets.AssetsArray[sc.assetsIndex];
                    var h = entity.GetHitbox();
                    h.x -= Variables.Camera.x;
                    h.y -= Variables.Camera.y;

                    if (sc.animated)
                    {
                        SDL.SDL_Rect s = new SDL.SDL_Rect()
                        {
                            x = sc.spriteX + (sc.spriteCol * sc.spriteXDiff),
                            y = sc.spriteY + (sc.spriteRow * sc.spriteYDiff),
                            h = sc.spriteHeight,
                            w = sc.spriteWidth,
                        };
                        if (entity.GetFacing() == 1)
                        {
                            SDL.SDL_RenderCopyF(Variables.Renderer, t, ref s, ref h);
                        }
                        else
                        {
                            SDL.SDL_RenderCopyExF(Variables.Renderer, t, ref s, ref h, 0, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL);
                        }
                    }
                    else
                    {
                        SDL.SDL_RenderCopyF(Variables.Renderer, t, IntPtr.Zero, ref h);
                    }
                }


                Entity player = EntityManager.GetPlayer();
                SpriteComponent spriteComponent = player.GetComponent<SpriteComponent>();
                var texture = Assets.AssetsArray[spriteComponent.assetsIndex];
                SDL.SDL_FRect hitbox = player.GetHitbox();
                SDL.SDL_Rect src = new SDL.SDL_Rect()
                {
                    x = spriteComponent.spriteX + (spriteComponent.spriteCol * spriteComponent.spriteXDiff),
                    y = spriteComponent.spriteY + (spriteComponent.spriteRow * spriteComponent.spriteYDiff),
                    h = spriteComponent.spriteHeight,
                    w = spriteComponent.spriteWidth,
                };

                hitbox.x -= Variables.Camera.x;
                hitbox.y -= Variables.Camera.y;
                if (player.GetFacing() == 1)
                {
                    SDL.SDL_RenderCopyF(Variables.Renderer, texture, ref src, ref hitbox);
                }
                else
                {
                    SDL.SDL_RenderCopyExF(Variables.Renderer, texture, ref src, ref hitbox, 0, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL);
                }



                // Draw fps to screen
                Draw.DrawFps();
                SDL.SDL_RenderPresent(Variables.Renderer);


            }

        }
        public static void CleanUp()
        {
            for (int i = 0; i < Assets.Index; i++)
            {
                SDL.SDL_DestroyTexture(Assets.AssetsArray[i]);
            }
            SDL.SDL_DestroyRenderer(Variables.Renderer);
            SDL.SDL_DestroyWindow(Variables.Window);
            SDL_mixer.Mix_Quit();
            SDL.SDL_Quit();

        }
    }
}