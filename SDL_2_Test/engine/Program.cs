using SDL2;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SDL_2_Test.engine
{
    class Program
    {
        public static void Main(string[] args)
        {
            Setup();

            Variables.Running = true;



            Stopwatch sw;

            // Nanoseconds
            double elapsed = 1;
            // Nanoseconds
            float nanosecondsSum = 0;
            int frameSum = 0;
            int sleepMiliseconds;


            int size;


            while (Variables.Running)
            {
                // Start counting
                sw = Stopwatch.StartNew();




                // Main Game Loop Functions Start
                PollEvents();
                IntPtr state = SDL.SDL_GetKeyboardState(out size);
                Marshal.Copy(state, Variables.KeyArray, 0, size);
                Physics.Update(elapsed);
                Camera.Update();
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
                if (nanosecondsSum >= 1000000000)
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


            }




            CleanUp();

            /// <summary>
            /// Setup all of the SDL resources we'll need to display a window.
            /// </summary>
            void Setup()
            {
                // Player init
                Variables.Entities.Add(new Player());

                // Obstacles init

                for (int i = 0; i < 1; i++)
                {
                    var obstacle = new Obstacle();
                    Variables.Entities.Add(obstacle);
                }


                var obs = new Obstacle();
                obs.SetHitbox(300, -300);
                Variables.Entities.Add(obs);

                var os = new Obstacle();
                os.SetHitbox(150, -150);
                Variables.Entities.Add(os);


                // Fruit init

                for (int i = 0; i < 40; i++)
                {
                    var fruit = new Fruit();
                    Variables.Entities.Add(fruit);
                }




                // Initilizes SDL.
                if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
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


                SDL.SDL_SetRenderDrawBlendMode(Variables.Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
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


                var color = new SDL.SDL_Color();


                //Render obstacles
                List<Entity> list = EntityManager.GetAll<Obstacle>();
                foreach (Entity obstacle in list)
                {
                    for (int j = 0; j < obstacle.GetShapesCount(); j++)
                    {
                        var shape = obstacle.GetShape()[j].Item1;
                        shape.x -= Variables.Camera.x;
                        shape.y -= Variables.Camera.y;
                        color = obstacle.GetShape()[j].Item2;
                        SDL.SDL_SetRenderDrawColor(Variables.Renderer, color.r, color.g, color.b, color.a);
                        SDL.SDL_RenderFillRectF(Variables.Renderer, ref shape);
                    }
                }

                // Render fruit
                list = EntityManager.GetAll<Fruit>();
                foreach (Entity fruit in list)
                {
                    for (int j = 0; j < fruit.GetShapesCount(); j++)
                    {
                        var shape = fruit.GetShape()[j].Item1;
                        shape.x -= Variables.Camera.x;
                        shape.y -= Variables.Camera.y;
                        color = fruit.GetShape()[j].Item2;
                        SDL.SDL_SetRenderDrawColor(Variables.Renderer, color.r, color.g, color.b, color.a);
                        SDL.SDL_RenderFillRectF(Variables.Renderer, ref shape);
                    }
                }

                // Render player
                Entity player = EntityManager.GetPlayer();
                SDL.SDL_SetRenderDrawColor(Variables.Renderer, color.r, color.g, color.b, color.a);
                for (int i = 0; i < player.GetShapesCount(); i++)
                {
                    var shape = player.GetShape()[i].Item1;
                    shape.x -= Variables.Camera.x;
                    shape.y -= Variables.Camera.y;
                    color = player.GetShape()[i].Item2;
                    SDL.SDL_SetRenderDrawColor(Variables.Renderer, color.r, color.g, color.b, color.a);
                    SDL.SDL_RenderFillRectF(Variables.Renderer, ref shape);
                }

                // SDL.SDL_RenderFillRect(renderer, ref rect5test.rectangle);
                // Switches out the currently presented render surface with the one we just did work on.

                // Draw fps to screen
                Draw.DrawFps();
                SDL.SDL_RenderPresent(Variables.Renderer);

            }

            /// <summary>
            /// Clean up the resources that were created.
            /// </summary>
            void CleanUp()
            {
                SDL.SDL_DestroyRenderer(Variables.Renderer);
                SDL.SDL_DestroyWindow(Variables.Window);
                SDL.SDL_Quit();
            }
        }
    }
}